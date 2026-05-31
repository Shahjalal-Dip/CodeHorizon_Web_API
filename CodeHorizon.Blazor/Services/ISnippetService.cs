using CodeHorizon.Blazor.Models.Common;
using CodeHorizon.Blazor.Models.Snippets;

namespace CodeHorizon.Blazor.Services;

public interface ISnippetService
{
    Task<PaginatedResponse<SnippetResponse>> GetPublicAsync(SnippetFilter filter, CancellationToken ct = default);
    Task<PaginatedResponse<SnippetResponse>> GetMySnippetsAsync(SnippetFilter filter, CancellationToken ct = default);
    Task<SnippetResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SnippetResponse?> CreateAsync(SnippetRequest request, CancellationToken ct = default);
    Task<SnippetResponse?> UpdateAsync(Guid id, SnippetRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
