using CodeHorizon.Application.DTOs.Auth;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Application.Services;
using CodeHorizon.Core.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;

namespace CodeHorizon.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "imdipYourFatherThatIsAtLeast32CharactersLongForTesting"},
                {"Jwt:Issuer", "CodeHorizon"},
                {"Jwt:Audience", "CodeHorizonUsers"},
                {"Jwt:ExpiresInMinutes", "60"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(_userRepositoryMock.Object, _configuration);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldRegisterUser()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Test123!",
                FullName = "Test User"
            };

            _userRepositoryMock
                .Setup(x => x.EmailExistsAsync(registerDto.Email))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(registerDto.Username))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            //Act
            var result = _authService.RegisterAsync(registerDto);

            //Assert
            result.Should().NotBeNull();
            
        }
        }
}
