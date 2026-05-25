using CodeHorizon.Blazor.Helpers;
using CodeHorizon.Blazor.Models.Auth;
using CodeHorizon.Blazor.Providers;
using CodeHorizon.Blazor.Utils;

namespace CodeHorizon.Blazor.Services;

public class AuthService(
    IApiClient api,
    ILocalStorageService storage,
    CustomAuthStateProvider authStateProvider) : IAuthService
{
    public bool IsAuthenticated { get; private set; }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var result = await api.PostAuthAsync<AuthResponse>("login", request);
        if (result is not null)
            await PersistAuthAsync(result);
        return result;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var result = await api.PostAuthAsync<AuthResponse>("register", request);
        if (result is not null)
            await PersistAuthAsync(result);
        return result;
    }

    public async Task LogoutAsync()
    {
        await storage.RemoveItemAsync(LocalStorageKeys.AuthToken);
        IsAuthenticated = false;
        await authStateProvider.MarkUserAsLoggedOut();
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = await storage.GetItemAsync<string>(LocalStorageKeys.AuthToken);
        if (string.IsNullOrWhiteSpace(token) || JwtHelper.IsExpired(token))
        {
            await LogoutAsync();
            return null;
        }

        IsAuthenticated = true;
        return token;
    }

    private async Task PersistAuthAsync(AuthResponse response)
    {
        await storage.SetItemAsync(LocalStorageKeys.AuthToken, response.Token);
        IsAuthenticated = true;
        await authStateProvider.MarkUserAsAuthenticated(response.Token);
    }
}
