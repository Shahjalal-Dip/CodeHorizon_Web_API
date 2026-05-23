using System.Security.Claims;
using CodeHorizon.Blazor.Helpers;
using CodeHorizon.Blazor.Services;
using CodeHorizon.Blazor.Utils;
using Microsoft.AspNetCore.Components.Authorization;

namespace CodeHorizon.Blazor.Providers;

public class CustomAuthStateProvider(ILocalStorageService storage) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.GetItemAsync<string>(LocalStorageKeys.AuthToken);
        if (string.IsNullOrWhiteSpace(token) || JwtHelper.IsExpired(token))
            return Anonymous;

        var principal = JwtHelper.ParseClaims(token);
        if (principal is null)
            return Anonymous;

        return new AuthenticationState(principal);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        var principal = JwtHelper.ParseClaims(token) ?? new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        await Task.CompletedTask;
    }

    public Task MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        return Task.CompletedTask;
    }
}
