using CodeHorizon.Application.Interfaces;
using CodeHorizon.Core.Entities;
using CodeHorizon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Infrastructure.Repositories
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly CodeHorizonDbContext _context;
        public BookmarkRepository(CodeHorizonDbContext context)
        {
            _context = context;
        }
        public async Task<Bookmark> CreateAsync(Bookmark bookmark)
        {
            bookmark.Id = Guid.NewGuid();
            bookmark.CreatedAt = DateTime.UtcNow;

            await _context.Bookmarks.AddAsync(bookmark);
            await _context.SaveChangesAsync();
            return bookmark;
        }

        public async Task DeleteAsync(Bookmark bookmark)
        {
            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }

        public async Task<Bookmark?> GetByIdAsync(Guid id)
        {
            return await _context.Bookmarks
                .Include(b=>b.User)
                .Include(b=>b.Snippet)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bookmark?> GetByUserAndSnippetAsync(Guid userId, Guid snippetId)
        {
            return await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.SnippetId == snippetId);
        }

        public async Task<int> GetSnippetBookmarkCountAsync(Guid snippetId)
        {
            return await _context.Bookmarks.CountAsync(b => b.SnippetId == snippetId);
        }

        public async Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(Guid userId, int page, int pageSize)
        {
            return await _context.Bookmarks
                .Include(b => b.Snippet)
                   .ThenInclude(s => s.Author)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserBookmarksCountAsync(Guid userId)
        {
            return await _context.Bookmarks.CountAsync(b => b.UserId == userId);
        }

        public async Task<bool> IsBookmarkedAsync(Guid userId, Guid snippetId)
        {
            return await _context.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.SnippetId == snippetId);
        }
    }
}
