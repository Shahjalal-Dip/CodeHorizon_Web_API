using CodeHorizon.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<Bookmark?> GetByIdAsync(Guid id);
        Task<Bookmark?> GetByUserAndSnippetAsync(Guid userId, Guid snippetId);
        Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(Guid userId, int page, int pageSize);
        Task<int> GetUserBookmarksCountAsync(Guid userId);
        Task<Bookmark> CreateAsync(Bookmark bookmark);
        Task DeleteAsync(Bookmark bookmark);
        Task<bool> IsBookmarkedAsync(Guid userId, Guid snippetId);
        Task<int> GetSnippetBookmarkCountAsync(Guid snippetId);
    }
}
