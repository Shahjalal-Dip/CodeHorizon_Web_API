using System.Threading.Tasks;
using CodeHorizon.Application.DTOs.Auth;

namespace CodeHorizon.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}