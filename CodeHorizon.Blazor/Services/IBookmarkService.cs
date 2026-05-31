using CodeHorizon.Blazor.Models.Bookmarks;
using CodeHorizon.Blazor.Models.Common;

namespace CodeHorizon.Blazor.Services;

public interface IBookmarkService
{
    Task<PaginatedResponse<BookmarkResponse>> GetMyBookmarksAsync(int page, int pageSize, CancellationToken ct = default);
    Task<BookmarkResponse?> AddAsync(Guid snippetId, CancellationToken ct = default);
    Task<bool> RemoveAsync(Guid snippetId, CancellationToken ct = default);
    Task<bool> IsBookmarkedAsync(Guid snippetId, CancellationToken ct = default);
}
