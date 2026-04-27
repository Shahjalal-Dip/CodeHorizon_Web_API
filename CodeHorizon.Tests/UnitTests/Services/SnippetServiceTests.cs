using CodeHorizon.Application.DTOs.Snippet;
using CodeHorizon.Application.Interfaces;
using CodeHorizon.Application.Jobs;
using CodeHorizon.Application.Services;
using CodeHorizon.Core.Entities;
using CodeHorizon.Core.Exceptions;
using CodeHorizon.Tests.Helpers;
using FluentAssertions;
using Hangfire;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Tests.UnitTests.Services
{
    public class SnippetServiceTests
    {
        public readonly Mock<ISnippetRepository> _snippetRepositoryMock;
        public readonly Mock<ITagRepository> _tagRepositoryMock;
        public readonly Mock<IUserRepository> _userRepositoryMock;
        public readonly Mock<ICacheService> _cacheServiceMock;
        public readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
        public readonly Mock<ISnippetJobs> _snippetJobsMock;
        public readonly SnippetService _snippetService;


        public SnippetServiceTests()
        {
            _snippetRepositoryMock = new Mock<ISnippetRepository>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _snippetJobsMock = new Mock<ISnippetJobs>();


            _snippetService = new SnippetService(
                _snippetRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _userRepositoryMock.Object,
                _cacheServiceMock.Object,
                _backgroundJobClientMock.Object,
                _snippetJobsMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnSnippet()
        {
            //Arange
            var user = TestDataFactory.CreateUser();
            var snippet = TestDataFactory.CreateSnippet(user);
            var snippetId = snippet.Id;

            _snippetRepositoryMock
                .Setup(x => x.GetByIdAsync(snippetId))
                .ReturnsAsync(snippet);

            _cacheServiceMock
                .Setup(x => x.GetAsync<SnippetResponseDto>(It.IsAny<string>()))
                .ReturnsAsync((SnippetResponseDto)null);

            //Act
            var result = await _snippetService.GetByIdAsync(snippetId, user.Id);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(snippet.Id);
            result.Title.Should().Be(snippet.Title);

            _snippetRepositoryMock.Verify(x => x.GetByIdAsync(snippetId), Times.Once());
            _cacheServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<SnippetResponseDto>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
        {
            //Arrange
            var invalidId = Guid.NewGuid();

            _snippetRepositoryMock
                .Setup(x => x.GetByIdAsync(invalidId))
                .ReturnsAsync((Snippet)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _snippetService.GetByIdAsync(invalidId, null));
        }

        [Fact]
        public async Task GetByIdAsync_WithPrivateSnippet_AndUnauthorizedUser_ShouldThrowForbiddenException()
        {
            // Arrange
            var author = TestDataFactory.CreateUser();
            var snippet = TestDataFactory.CreateSnippet(author, isPublic: false);
            var otherUser = TestDataFactory.CreateUser();

            _snippetRepositoryMock
                .Setup(x => x.GetByIdAsync(snippet.Id))
                .ReturnsAsync(snippet);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _snippetService.GetByIdAsync(snippet.Id, otherUser.Id));
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateSnippet()
        {
            // Arrange
            var author = TestDataFactory.CreateUser();
            var createDto = new CreateSnippetDto
            {
                Title = "Test Snippet",
                Content = "Console.WriteLine('Hello World');",
                Description = "A test snippet",
                Language = "csharp",
                IsPublic = true,
                Tags = new List<string> { "csharp", "test" }
            };

            var tags = TestDataFactory.CreateTags(new List<string> { "csharp", "test" });

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(author.Id))
                .ReturnsAsync(author);

            _tagRepositoryMock
                .Setup(x => x.GetOrCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(tags);

            _snippetRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Snippet>()))
                .ReturnsAsync((Snippet s) => s);

            // Act
            var result = await _snippetService.CreateAsync(createDto, author.Id);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(createDto.Title);
            result.Language.Should().Be(createDto.Language);

            _snippetRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Snippet>()), Times.Once);

        }

        [Fact]
        public async Task DeleteAsync_WithValidUser_ShouldDeleteSnippet()
        {
            // Arrange
            var author = TestDataFactory.CreateUser();
            var snippet = TestDataFactory.CreateSnippet(author);

            _snippetRepositoryMock
                .Setup(x => x.GetByIdAsync(snippet.Id))
                .ReturnsAsync(snippet);

            // Act
            await _snippetService.DeleteAsync(snippet.Id, author.Id);

            // Assert
            _snippetRepositoryMock.Verify(x => x.DeleteAsync(snippet), Times.Once);
            _cacheServiceMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithWrongUser_ShouldThrowForbiddenException()
        {
            // Arrange
            var author = TestDataFactory.CreateUser();
            var snippet = TestDataFactory.CreateSnippet(author);
            var wrongUser = TestDataFactory.CreateUser();

            _snippetRepositoryMock
                .Setup(x => x.GetByIdAsync(snippet.Id))
                .ReturnsAsync(snippet);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _snippetService.DeleteAsync(snippet.Id, wrongUser.Id));
        }
    }
}
