using CodeHorizon.Application.DTOs.Snippet;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Core.Entities;
using CodeHorizon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CodeHorizon.Infrastructure.Repositories
{
    public class SnippetRepository : ISnippetRepository
    {
        private readonly CodeHorizonDbContext _context;

        public SnippetRepository(CodeHorizonDbContext context)
        {
            _context = context;
        }

        public async Task<Snippet?> GetByIdAsync(Guid id)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Snippet>> GetAllAsync(int page, int pageSize, string? language, string? search)
        {
            var query = _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.IsPublic);

            if (!string.IsNullOrEmpty(language))
            {
                query = query.Where(s => s.Language.ToLower() == language.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    s.Description.Contains(search) ||
                    s.Content.Contains(search));
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Snippet>> GetByUserAsync(Guid userId, int page, int pageSize)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.AuthorId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? language, string? search)
        {
            var query = _context.Snippets.Where(s => s.IsPublic);

            if (!string.IsNullOrEmpty(language))
            {
                query = query.Where(s => s.Language.ToLower() == language.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    s.Description.Contains(search) ||
                    s.Content.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<Snippet> CreateAsync(Snippet snippet)
        {
            snippet.Id = Guid.NewGuid();
            snippet.CreatedAt = DateTime.UtcNow;

            await _context.Snippets.AddAsync(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public async Task UpdateAsync(Snippet snippet)
        {
            snippet.UpdatedAt = DateTime.UtcNow;
            _context.Snippets.Update(snippet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Snippet snippet)
        {
            _context.Snippets.Remove(snippet);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Snippets.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Snippet>> GetByTagAsync(string tagName, int page, int pageSize)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.SnippetTags.Any(st => st.Tag.Name.ToLower() == tagName.ToLower()))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(Guid id)
        {
            var snippet = await _context.Snippets.FindAsync(id);
            if (snippet != null)
            {
                snippet.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Snippet>> GetAllFilteredAsync(SnippetFilterDto filterDto, int page, int pageSize)
        {
            var query = _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .AsQueryable();

            if(filterDto.IsPublic.HasValue)
            {
                query = query.Where(s => s.IsPublic == filterDto.IsPublic.Value);
            }
            //else
            //{
            //    query = query.Where(s => s.IsPublic);
            //}

            if (!string.IsNullOrEmpty(filterDto.Language))
            {
                query = query.Where(s => s.Language.ToLower() == filterDto.Language.ToLower());
            }

            if (!string.IsNullOrEmpty(filterDto.Search))
            {
                query = query.Where(s =>
                    s.Title.Contains(filterDto.Search) ||
                    s.Description.Contains(filterDto.Search) ||
                    s.Content.Contains(filterDto.Search));
            }

            if(!string.IsNullOrEmpty(filterDto.Tag))
            {
                query = query
                    .Where(s => s.SnippetTags
                    .Any(st => st.Tag.Name.ToLower() == filterDto.Tag.ToLower()));
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= filterDto   .FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= filterDto.ToDate.Value);
            }

            if (filterDto.AuthorId.HasValue)
            {
                query = query.Where(s => s.AuthorId == filterDto.AuthorId.Value);
            }

            // sorting
            query = filterDto.SortBy?.ToLower() switch
            {
                "views" => filterDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(s => s.ViewCount)
                    : query.OrderByDescending(s => s.ViewCount),
                "bookmarks" => filterDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(s => s.BookmarkCount)
                    : query.OrderByDescending(s => s.BookmarkCount),
                _ => filterDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(s => s.CreatedAt)
                    : query.OrderByDescending(s => s.CreatedAt) // Default to newest first
            };

            //pagination
            return await query
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();

        }

        public async Task<int> GetTotalCountFilteredAsync(SnippetFilterDto filterDto)
        {
            var query = _context.Snippets.AsQueryable();

            if (filterDto.IsPublic.HasValue)
            {
                query = query.Where(s => s.IsPublic == filterDto.IsPublic.Value);
            }
            //else
            //{
            //    query = query.Where(s => s.IsPublic);
            //}

            if (!string.IsNullOrEmpty(filterDto.Language))
            {
                query = query.Where(s => s.Language.ToLower() == filterDto.Language.ToLower());
            }

            if (!string.IsNullOrEmpty(filterDto.Search))
            {
                query = query.Where(s =>
                    s.Title.Contains(filterDto.Search) ||
                    s.Description.Contains(filterDto.Search) ||
                    s.Content.Contains(filterDto.Search));
            }

            if (!string.IsNullOrEmpty(filterDto.Tag))
            {
                query = query.Where(s =>
                    s.SnippetTags.Any(st => st.Tag.Name.ToLower() == filterDto.Tag.ToLower()));
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= filterDto.FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= filterDto.ToDate.Value);
            }

            if (filterDto.AuthorId.HasValue)
            {
                query = query.Where(s => s.AuthorId == filterDto.AuthorId.Value);
            }

            return await query.CountAsync();
        }
    }
}