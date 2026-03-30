using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeHorizon.Core.Entities;

namespace CodeHorizon.Application.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByNameAsync(string name);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames);
        Task<Tag> CreateAsync(Tag tag);
        Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string name);
        Task<int> GetSnippetCountAsync(Guid tagId);
    }
}