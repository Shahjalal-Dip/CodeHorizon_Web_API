using CodeHorizon.Blazor.Models.Auth;

namespace CodeHorizon.Blazor.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> Login(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
        Task Logout();
        Task<bool> IsAuthenticated();
        Task<string?> GetToken();
        Task<AuthResponse?> GetCurrentUser();
    }
}
