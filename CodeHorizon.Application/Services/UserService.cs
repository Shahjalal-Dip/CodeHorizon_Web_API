using CodeHorizon.Application.DTOs.User;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Core.Entities;
using CodeHorizon.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookmarkRepository _bookmarkRepository;
        public UserService(IUserRepository userRepository, IBookmarkRepository bookmarkRepository) 
        {
            _userRepository = userRepository;
            _bookmarkRepository = bookmarkRepository;
        }
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                throw new NotFoundException("User not Found", userId.ToString());
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new Exception("Current password is incorrect");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new Exception("New password and confirmation do not match");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                //throw new Exception("User not found");
                throw new NotFoundException("User not Found", userId.ToString());
            }

            var snippetsCount = await _userRepository.GetUserSnippetsCountAsync(userId);
            var bookmarksCount = await _bookmarkRepository.GetUserBookmarksCountAsync(userId);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                SnippetsCount = snippetsCount,
                BookmarksCount = bookmarksCount
            };
        }

        public async Task<UserProfileDto> GetProfileByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameWithDetailsAsync(username);
            if(user == null)
            {
                //throw new Exception("User not found");
                throw new NotFoundException("User not Found", username);
            }

            return new UserProfileDto
            {
                Id = user.Id, 
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                SnippetsCount = user.Snippets.Count,
                BookmarksCount = user.Bookmarks.Count
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not Found", userId.ToString());
            }

            // Update only provided fields
            if (updateDto.Bio != null)
                user.Bio = updateDto.Bio;

            if (updateDto.ProfilePictureUrl != null)
                user.ProfilePictureUrl = updateDto.ProfilePictureUrl;

            if (updateDto.FullName != null)
                user.FullName = updateDto.FullName;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return await GetProfileAsync(userId);

        }
    }
}
