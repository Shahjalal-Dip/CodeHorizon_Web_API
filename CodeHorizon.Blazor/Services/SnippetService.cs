using CodeHorizon.Blazor.Models.Common;
using CodeHorizon.Blazor.Models.Snippets;

namespace CodeHorizon.Blazor.Services;

public class SnippetService(IApiClient api, StateContainer state) : ISnippetService
{
    public Task<PaginatedResponse<SnippetResponse>> GetPublicAsync(SnippetFilter filter, CancellationToken ct = default) =>
        api.GetAsync<PaginatedResponse<SnippetResponse>>($"snippets?{filter.ToQueryString()}", ct)!;

    public Task<PaginatedResponse<SnippetResponse>> GetMySnippetsAsync(SnippetFilter filter, CancellationToken ct = default)
    {
        var query = $"page={filter.Page}&pageSize={filter.PageSize}";
        if (!string.IsNullOrWhiteSpace(filter.Language))
            query += $"&language={Uri.EscapeDataString(filter.Language)}";
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query += $"&search={Uri.EscapeDataString(filter.Search)}";
        return api.GetAsync<PaginatedResponse<SnippetResponse>>($"snippets/my-snippets?{query}", ct)!;
    }

    public Task<SnippetResponse?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        api.GetAsync<SnippetResponse>($"snippets/{id}", ct);

    public async Task<SnippetResponse?> CreateAsync(SnippetRequest request, CancellationToken ct = default)
    {
        state.InvalidateSnippets();
        return await api.PostAsync<SnippetResponse>("snippets", request, ct);
    }

    public async Task<SnippetResponse?> UpdateAsync(Guid id, SnippetRequest request, CancellationToken ct = default)
    {
        state.InvalidateSnippets();
        return await api.PutAsync<SnippetResponse>($"snippets/{id}", request, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        state.InvalidateSnippets();
        return await api.DeleteAsync($"snippets/{id}", ct);
    }
}
