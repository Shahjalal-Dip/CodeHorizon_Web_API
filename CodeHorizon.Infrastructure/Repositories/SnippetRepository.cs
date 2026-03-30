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
    public class SnippetRepository : ISnippetRepository
    {
        private readonly CodeHorizonDbContext _context;

        public SnippetRepository(CodeHorizonDbContext context)
        {
            _context = context;
        }

        public async Task<Snippet?> GetByIdAsync(Guid id)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Snippet>> GetAllAsync(int page, int pageSize, string? language, string? search)
        {
            var query = _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.IsPublic);

            if (!string.IsNullOrEmpty(language))
            {
                query = query.Where(s => s.Language.ToLower() == language.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    s.Description.Contains(search) ||
                    s.Content.Contains(search));
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Snippet>> GetByUserAsync(Guid userId, int page, int pageSize)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.AuthorId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? language, string? search)
        {
            var query = _context.Snippets.Where(s => s.IsPublic);

            if (!string.IsNullOrEmpty(language))
            {
                query = query.Where(s => s.Language.ToLower() == language.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    s.Description.Contains(search) ||
                    s.Content.Contains(search));
            }

            return await query.CountAsync();
        }

        public async Task<Snippet> CreateAsync(Snippet snippet)
        {
            snippet.Id = Guid.NewGuid();
            snippet.CreatedAt = DateTime.UtcNow;

            await _context.Snippets.AddAsync(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public Task UpdateAsync(Snippet snippet)
        {
            snippet.UpdatedAt = DateTime.UtcNow;
            _context.Snippets.Update(snippet);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Snippet snippet)
        {
            _context.Snippets.Remove(snippet);
            _context.SaveChangesAsync();
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Snippets.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Snippet>> GetByTagAsync(string tagName, int page, int pageSize)
        {
            return await _context.Snippets
                .Include(s => s.Author)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .Where(s => s.SnippetTags.Any(st => st.Tag.Name.ToLower() == tagName.ToLower()))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(Guid id)
        {
            var snippet = await _context.Snippets.FindAsync(id);
            if (snippet != null)
            {
                snippet.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}