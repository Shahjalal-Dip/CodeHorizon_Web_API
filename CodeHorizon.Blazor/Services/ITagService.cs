using CodeHorizon.Blazor.Models.Tags;

namespace CodeHorizon.Blazor.Services;

public interface ITagService
{
    Task<List<TagResponse>> GetAllAsync(CancellationToken ct = default);
    Task<List<TagResponse>> SearchAsync(string query, CancellationToken ct = default);
}
