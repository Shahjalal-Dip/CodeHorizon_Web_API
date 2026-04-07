using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodeHorizon.Core.Entities;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Infrastructure.Data;

namespace CodeHorizon.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CodeHorizonDbContext _context;

        public UserRepository(CodeHorizonDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            return user;
        }

        public Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetUserSnippetsCountAsync(Guid userId)
        {
            return await _context.Snippets
                .CountAsync(s=>s.AuthorId == userId);
        }

        public async Task<int> GetUserBookmarksCountAsync(Guid userId)
        {
            return await _context.Bookmarks
                .CountAsync(b => b.UserId == userId);
        }

        public async Task<User?> GetByUsernameWithDetailsAsync(string username)
        {
            return await _context.Users
                .Where(u => u.Username == username)
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    Bio = u.Bio,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    CreatedAt = u.CreatedAt,
                    IsActive = u.IsActive,
                    Snippets = u.Snippets.Take(5).ToList(),
                    Bookmarks = u.Bookmarks.Take(5).ToList()

                })
                .FirstOrDefaultAsync();
        }
    }
}