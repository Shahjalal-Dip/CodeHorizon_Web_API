using CodeHorizon.Blazor.Models.Auth;

namespace CodeHorizon.Blazor.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    bool IsAuthenticated { get; }
}
