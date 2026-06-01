using CodeHorizon.Blazor.Models.Users;

namespace CodeHorizon.Blazor.Services;

public class UserService(IApiClient api, StateContainer state) : IUserService
{
    public async Task<UserProfileResponse?> GetMyProfileAsync(CancellationToken ct = default)
    {
        if (state.CachedProfile is not null)
            return state.CachedProfile;

        var profile = await api.GetAsync<UserProfileResponse>("users/profile", ct);
        if (profile is not null)
            state.CachedProfile = profile;
        return profile;
    }

    public Task<UserProfileResponse?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        api.GetAsync<UserProfileResponse>($"users/profile/{Uri.EscapeDataString(username)}", ct);

    public async Task<UserProfileResponse?> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default)
    {
        state.InvalidateProfile();
        return await api.PutAsync<UserProfileResponse>("users/profile", request, ct);
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        var result = await api.PostAsync<object>("users/change-password", request, ct);
        return result is not null || true;
    }
}
