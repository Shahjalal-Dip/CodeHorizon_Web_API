using CodeHorizon.Application.DTOs.User;
using CodeHorizon.Application.Helpers;
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
        private readonly ICacheService _cacheService;
        public UserService(IUserRepository userRepository, IBookmarkRepository bookmarkRepository, ICacheService cacheService) 
        {
            _userRepository = userRepository;
            _bookmarkRepository = bookmarkRepository;
            _cacheService = cacheService;
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
            var cachekey = CacheKeys.UserProfileKey(userId);
            var cachedProfile = await _cacheService.GetAsync<UserProfileDto>(cachekey);
            if (cachedProfile != null)
            {
                return cachedProfile;
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                //throw new Exception("User not found");
                throw new NotFoundException("User not Found", userId.ToString());
            }

            var snippetsCount = await _userRepository.GetUserSnippetsCountAsync(userId);
            var bookmarksCount = await _bookmarkRepository.GetUserBookmarksCountAsync(userId);

            var userProfile = new UserProfileDto
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

            await _cacheService.SetAsync(cachekey, userProfile, TimeSpan.FromMinutes(30));

            return userProfile;
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
            var cachekey = CacheKeys.UserProfileKey(userId);
            var cachedProfile = await _cacheService.GetAsync<UserProfileDto>(cachekey);


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


            if (cachedProfile != null)
            {
                cachedProfile.Bio = updateDto.Bio ?? cachedProfile.Bio;
                cachedProfile.ProfilePictureUrl = updateDto.ProfilePictureUrl ?? cachedProfile.ProfilePictureUrl;
                cachedProfile.FullName = updateDto.FullName ?? cachedProfile.FullName;

                await _cacheService.SetAsync(cachekey, cachedProfile, TimeSpan.FromMinutes(30));
            }

            await _userRepository.SaveChangesAsync();

            return await GetProfileAsync(userId);
        }
    }
}
