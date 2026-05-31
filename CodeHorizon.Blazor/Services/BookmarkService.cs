using CodeHorizon.Blazor.Models.Bookmarks;
using CodeHorizon.Blazor.Models.Common;

namespace CodeHorizon.Blazor.Services;

public class BookmarkService(IApiClient api) : IBookmarkService
{
    public Task<PaginatedResponse<BookmarkResponse>> GetMyBookmarksAsync(int page, int pageSize, CancellationToken ct = default) =>
        api.GetAsync<PaginatedResponse<BookmarkResponse>>($"bookmarks?page={page}&pageSize={pageSize}", ct)!;

    public Task<BookmarkResponse?> AddAsync(Guid snippetId, CancellationToken ct = default) =>
        api.PostAsync<BookmarkResponse>($"bookmarks/{snippetId}", null, ct);

    public Task<bool> RemoveAsync(Guid snippetId, CancellationToken ct = default) =>
        api.DeleteAsync($"bookmarks/{snippetId}", ct);

    public async Task<bool> IsBookmarkedAsync(Guid snippetId, CancellationToken ct = default)
    {
        var result = await api.GetAsync<BookmarkCheckResponse>($"bookmarks/check/{snippetId}", ct);
        return result?.IsBookmarked ?? false;
    }

    private sealed class BookmarkCheckResponse
    {
        public bool IsBookmarked { get; set; }
    }
}
