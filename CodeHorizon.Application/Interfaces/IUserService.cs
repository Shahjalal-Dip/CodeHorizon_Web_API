using CodeHorizon.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task<UserProfileDto> GetProfileByUsernameAsync(string username);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto updateDto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
    }
}
