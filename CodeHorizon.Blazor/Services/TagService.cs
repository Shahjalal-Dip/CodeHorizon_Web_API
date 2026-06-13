using CodeHorizon.Blazor.Models.Tags;

namespace CodeHorizon.Blazor.Services;

public class TagService(IApiClient api, StateContainer state) : ITagService
{
    public async Task<List<TagResponse>> GetAllAsync(CancellationToken ct = default)
    {
        if (state.IsTagsCacheValid && state.CachedTags is not null)
            return state.CachedTags;

        var tags = await api.GetAsync<List<TagResponse>>("tags", ct) ?? [];
        state.CachedTags = tags;
        state.TagsCacheTime = DateTime.UtcNow;
        return tags;
    }

    public async Task<List<TagResponse>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var tags = await api.GetAsync<List<TagResponse>>($"tags/search?q={Uri.EscapeDataString(query)}", ct);
        return tags ?? [];
    }
}
