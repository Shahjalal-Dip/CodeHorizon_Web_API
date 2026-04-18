using CodeHorizon.Application.DTOs.Auth;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Core.Entities;
using CodeHorizon.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeHorizon.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
           
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                //throw new Exception("Email already registered");
                throw new ConflictException("Email already registered");
            }

           
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                //throw new Exception("Username already taken");
                throw new ConflictException("Username already taken");
            }

            // Create password hash (in production, use BCrypt or similar)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create user
            var user = new User
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                FullName = registerDto.FullName,
                IsActive = true
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
            {
                //throw new Exception("Invalid credentials");
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new Exception("Account is disabled");
            }

            // Generate token
            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FullName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName,
                ExpiresAt = tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}