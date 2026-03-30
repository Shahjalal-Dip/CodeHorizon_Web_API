using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeHorizon.Core.Entities;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Infrastructure.Data;

namespace CodeHorizon.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly CodeHorizonDbContext _context;

        public TagRepository(CodeHorizonDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags
                .Include(t => t.SnippetTags)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> tagNames)
        {
            var lowerCaseNames = tagNames.Select(n => n.ToLower()).ToList();
            return await _context.Tags
                .Where(t => lowerCaseNames.Contains(t.Name.ToLower()))
                .ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            tag.Id = Guid.NewGuid();
            tag.Name = tag.Name.ToLower(); // Normalize to lowercase

            await _context.Tags.AddAsync(tag);
            return tag;
        }

        public async Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            var tags = new List<Tag>();
            var uniqueTagNames = tagNames
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct()
                .ToList();

            if (!uniqueTagNames.Any())
            {
                return tags;
            }

            // First, get existing tags
            var existingTags = await GetTagsByNamesAsync(uniqueTagNames);
            var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToHashSet();

            // Add existing tags to result
            tags.AddRange(existingTags);

            // Create non-existing tags
            var newTagNames = uniqueTagNames
                .Where(n => !existingTagNames.Contains(n.ToLower()))
                .ToList();

            foreach (var tagName in newTagNames)
            {
                var newTag = new Tag { Name = tagName.ToLower() };
                tags.Add(await CreateAsync(newTag));
            }

            // Save all new tags to database
            if (newTagNames.Any())
            {
                await _context.SaveChangesAsync();
            }

            return tags;
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await GetByIdAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Tags
                .AnyAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<int> GetSnippetCountAsync(Guid tagId)
        {
            return await _context.SnippetTags
                .CountAsync(st => st.TagId == tagId);
        }

        public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count = 10)
        {
            return await _context.Tags
                .OrderByDescending(t => t.SnippetTags.Count)
                .Take(count)
                .ToListAsync();
        }
    }
}