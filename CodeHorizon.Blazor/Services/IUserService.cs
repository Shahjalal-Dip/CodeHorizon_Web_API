using CodeHorizon.Blazor.Models.Users;

namespace CodeHorizon.Blazor.Services;

public interface IUserService
{
    Task<UserProfileResponse?> GetMyProfileAsync(CancellationToken ct = default);
    Task<UserProfileResponse?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<UserProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);
}
