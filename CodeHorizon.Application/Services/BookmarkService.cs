using CodeHorizon.Application.DTOs;
using CodeHorizon.Application.DTOs.Bookmark;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Core.Entities;
using CodeHorizon.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly ISnippetRepository _snippetRepository;
        public BookmarkService(IBookmarkRepository bookmarkRepository, ISnippetRepository snippetRepository)
        {
            _bookmarkRepository = bookmarkRepository;
            _snippetRepository = snippetRepository;
        }
        public async Task<BookmarkResponseDto> AddBookmarkAsync(Guid userId, Guid snippetId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(snippetId);
            if (snippet == null)
                throw new NotFoundException("Snippet", snippetId.ToString());

            var existingBookmark = await _bookmarkRepository.GetByUserAndSnippetAsync(userId, snippetId);
            if (existingBookmark != null)
                throw new ConflictException("Snippet already bookmarked");

            var bookmark = new Bookmark
            {
                UserId = userId,
                SnippetId = snippetId
            };

            await _bookmarkRepository.CreateAsync(bookmark);

            snippet.BookmarkCount = await _bookmarkRepository.GetSnippetBookmarkCountAsync(snippetId);
            await _snippetRepository.UpdateAsync(snippet);

            return new BookmarkResponseDto
            {
                Id = bookmark.Id,
                SnippetId = snippet.Id,
                SnippetTitle = snippet.Title,
                SnippetLanguage = snippet.Language,
                AuthorUsername = snippet.Author.Username,
                CreatedAt = bookmark.CreatedAt
            };
        }

        public async Task<int> GetBookmarkCountAsync(Guid snippetId)
        {
            return await _bookmarkRepository.GetSnippetBookmarkCountAsync(snippetId);
        }

        public async Task<PagedResultDto<BookmarkResponseDto>> GetUserBookmarksAsync(Guid userId, int page, int pageSize)
        {
            var bookmarks = await _bookmarkRepository.GetUserBookmarksAsync(userId, page, pageSize);
            var totalCount = await _bookmarkRepository.GetUserBookmarksCountAsync(userId);

            var data = bookmarks.Select(b => new BookmarkResponseDto
            {
                Id = b.Id,
                SnippetId = b.SnippetId,
                SnippetTitle = b.Snippet.Title,
                SnippetLanguage = b.Snippet.Language,
                AuthorUsername = b.Snippet.Author.Username,
                CreatedAt = b.CreatedAt
            }).ToList();

            return new PagedResultDto<BookmarkResponseDto>
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> IsBookmarkedAsync(Guid userId, Guid snippetId)
        {
            return await _bookmarkRepository.IsBookmarkedAsync(userId, snippetId);
        }

        public async Task RemoveBookmarkAsync(Guid userId, Guid snippetId)
        {
            var bookmark = await _bookmarkRepository.GetByUserAndSnippetAsync(userId, snippetId);
            if (bookmark == null)
                throw new NotFoundException("Bookmark", $"{userId}-{snippetId}");

            await _bookmarkRepository.DeleteAsync(bookmark);

            var snippet = await _snippetRepository.GetByIdAsync(snippetId);

            if(snippet != null)
            {
                snippet.BookmarkCount = await _bookmarkRepository.GetSnippetBookmarkCountAsync(snippetId);
                await _snippetRepository.UpdateAsync(snippet);
            }
        }
    }
}
