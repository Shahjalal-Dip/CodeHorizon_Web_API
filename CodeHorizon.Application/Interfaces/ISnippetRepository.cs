using CodeHorizon.Core.Entities;

namespace CodeHorizon.Application.Interfaces
{
    public interface ISnippetRepository
    {
        Task<Snippet?> GetByIdAsync(Guid id);
        Task<IEnumerable<Snippet>> GetAllAsync(int page, int pageSize, string? language, string? search);
        Task<IEnumerable<Snippet>> GetByUserAsync(Guid userId, int page, int pageSize);
        Task<int> GetTotalCountAsync(string? language, string? search);
        Task<Snippet> CreateAsync(Snippet snippet);
        Task UpdateAsync(Snippet snippet);
        Task DeleteAsync(Snippet snippet);
        Task<bool> ExistsAsync(Guid id);
        Task IncrementViewCountAsync(Guid id);
    }
}