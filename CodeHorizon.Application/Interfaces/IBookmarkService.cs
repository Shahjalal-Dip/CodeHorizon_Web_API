using CodeHorizon.Application.DTOs;
using CodeHorizon.Application.DTOs.Bookmark;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Interfaces
{
    public interface IBookmarkService
    {
        Task<PagedResultDto<BookmarkResponseDto>> GetUserBookmarksAsync(Guid userId, int page, int pageSize);
        Task<BookmarkResponseDto> AddBookmarkAsync(Guid userId, Guid snippetId);
        Task RemoveBookmarkAsync(Guid userId, Guid snippetId);
        Task<bool> IsBookmarkedAsync(Guid userId, Guid snippetId);
        Task<int>GetBookmarkCountAsync(Guid snippetId);
    }
}
