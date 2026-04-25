using CodeHorizon.Application.DTOs;
using CodeHorizon.Application.DTOs.Snippet;
using CodeHorizon.Application.Helpers;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Application.Jobs;
using CodeHorizon.Core.Entities;
using CodeHorizon.Core.Exceptions;
using Hangfire;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHorizon.Application.Services
{
    public class SnippetService : ISnippetService
    {
        private readonly ISnippetRepository _snippetRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ISnippetJobs _snippetJobs;

        public SnippetService(
            ISnippetRepository snippetRepository,
            ITagRepository tagRepository,
            IUserRepository userRepository,
            ICacheService cacheService,
            IBackgroundJobClient backgroundJobClient,
            ISnippetJobs snippetJobs)
        {
            _snippetRepository = snippetRepository;
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _cacheService = cacheService;
            _backgroundJobClient = backgroundJobClient;
            _snippetJobs = snippetJobs;
        }

        public async Task<SnippetResponseDto> GetByIdAsync(Guid id, Guid? currentUserId)
        {
            var cacheKey = CacheKeys.SnippetKey(id);
            var cachedSnippet = await _cacheService.GetAsync<SnippetResponseDto>(cacheKey);
            if (cachedSnippet != null)
            {
                return cachedSnippet;
            }

            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null)
            {
                //throw new Exception("Snippet not found");
                throw new NotFoundException("Snippet", id.ToString());
            }

            if (!snippet.IsPublic && (currentUserId == null || snippet.AuthorId != currentUserId))
            {
                //throw new Exception("Access denied");
                throw new ForbiddenException("You don't have access to this snippet");
            }

            // Increment view count
            // await _snippetRepository.IncrementViewCountAsync(id);

            var result = MapToResponseDto(snippet, currentUserId);

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            // Increment view count (don't wait for this)
            _ = Task.Run(() => _snippetRepository.IncrementViewCountAsync(id));

            return result;
        }

        public async Task<PagedResultDto<SnippetResponseDto>> GetAllFilteredAsync(SnippetFilterDto filter, int page, int pageSize, Guid? currentUserId)
        {
            var snippets = await _snippetRepository.GetAllFilteredAsync(filter, page, pageSize);
            var totalCount = await _snippetRepository.GetTotalCountFilteredAsync(filter);

            var data = snippets.Select(s => MapToResponseDto(s, currentUserId)).ToList();

            return new PagedResultDto<SnippetResponseDto>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<SnippetResponseDto> CreateAsync(CreateSnippetDto createDto, Guid authorId)
        {
            var user = await _userRepository.GetByIdAsync(authorId);
            if (user == null)
            {
                //throw new Exception("User not found");
                throw new NotFoundException("User Not found", authorId.ToString());
            }

            // Create or get tags
            var tags = await _tagRepository.GetOrCreateTagsAsync(createDto.Tags);

            // Create snippet
            var snippet = new Snippet
            {
                Title = createDto.Title,
                Content = createDto.Content,
                Description = createDto.Description,
                Language = createDto.Language,
                IsPublic = createDto.IsPublic,
                AuthorId = authorId,
                CodePreview = createDto.Content.Length > 100
                    ? createDto.Content.Substring(0, 100) + "..."
                    : createDto.Content
            };

            await _snippetRepository.CreateAsync(snippet);

            // Add tags
            foreach (var tag in tags)
            {
                snippet.SnippetTags.Add(new SnippetTag
                {
                    SnippetId = snippet.Id,
                    TagId = tag.Id
                });
            }

            await _snippetRepository.UpdateAsync(snippet);

            // Fire and forget background jobs
            _backgroundJobClient.Enqueue(() => _snippetJobs.GenerateSnippetPreviewAsync(snippet.Id));
            _backgroundJobClient.Enqueue(() => _snippetJobs.SendSnippetCreatedNotificationAsync(snippet.Id, authorId));

            // Schedule recurring job (if needed)
            RecurringJob.AddOrUpdate<ISnippetJobs>(
                "cleanup-old-snippets",
                job => job.CleanupOldSnippetsAsync(),
                Cron.Daily);

            //Invalidate snippet list cache
            await _cacheService.RemoveByPatternAsync("snippets_*");

            return await GetByIdAsync(snippet.Id, authorId);
        }

        public async Task<SnippetResponseDto> UpdateAsync(Guid id, CreateSnippetDto updateDto, Guid userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null)
            {
                //throw new Exception("Snippet not found");
                throw new NotFoundException("Snippet", id.ToString());
            }

            if (snippet.AuthorId != userId)
            {
                //throw new Exception("You don't have permission to update this snippet");
                throw new ForbiddenException("You don't have permission to modify this snippet");
            }

            // Update properties
            snippet.Title = updateDto.Title;
            snippet.Content = updateDto.Content;
            snippet.Description = updateDto.Description;
            snippet.Language = updateDto.Language;
            snippet.IsPublic = updateDto.IsPublic;
            snippet.CodePreview = updateDto.Content.Length > 100
                ? updateDto.Content.Substring(0, 100) + "..."
                : updateDto.Content;
            snippet.UpdatedAt = DateTime.UtcNow;

            // Update tags
            var newTags = await _tagRepository.GetOrCreateTagsAsync(updateDto.Tags);
            snippet.SnippetTags.Clear();
            foreach (var tag in newTags)
            {
                snippet.SnippetTags.Add(new SnippetTag
                {
                    SnippetId = snippet.Id,
                    TagId = tag.Id
                });
            }

            await _snippetRepository.UpdateAsync(snippet);

            // Invalidate specific snippet cache
            await _cacheService.RemoveAsync(CacheKeys.SnippetKey(id));
            await _cacheService.RemoveByPatternAsync("snippets_*");

            return await GetByIdAsync(snippet.Id, userId);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null)
            {
                //throw new Exception("Snippet not found");
                throw new NotFoundException("Snippet", id.ToString());
            }

            if (snippet.AuthorId != userId)
            {
                //throw new Exception("You don't have permission to delete this snippet");
                throw new ForbiddenException("You don't have permission to delete this snippet");
            }

            await _cacheService.RemoveAsync(CacheKeys.SnippetKey(id));
            await _cacheService.RemoveByPatternAsync("snippets_*");

            await _snippetRepository.DeleteAsync(snippet);
        }

        private SnippetResponseDto MapToResponseDto(Snippet snippet, Guid? currentUserId)
        {
            return new SnippetResponseDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Content = snippet.Content,
                Description = snippet.Description,
                Language = snippet.Language,
                CodePreview = snippet.CodePreview ?? string.Empty,
                ViewCount = snippet.ViewCount,
                BookmarkCount = snippet.BookmarkCount,
                IsPublic = snippet.IsPublic,
                CreatedAt = snippet.CreatedAt,
                UpdatedAt = snippet.UpdatedAt,
                AuthorId = snippet.AuthorId,
                AuthorUsername = snippet.Author?.Username ?? string.Empty,
                AuthorFullName = snippet.Author?.FullName ?? string.Empty,
                Tags = snippet.SnippetTags.Select(st => st.Tag.Name).ToList(),
                IsBookmarkedByCurrentUser = false 
            };
        }
    }
}