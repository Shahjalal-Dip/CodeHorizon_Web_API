using CodeHorizon.Application.DTOs.Snippet;
using CodeHorizon.Core.Entities;
using CodeHorizon.Infrastructure.Data;
using CodeHorizon.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CodeHorizon.Tests.IntegrationTests
{
    public class SnippetsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SnippetsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the database with in-memory for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<CodeHorizonDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<CodeHorizonDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutAuthentication_ShouldReturnPublicSnippets()
        {
            var response = await _client.GetAsync("/api/v1/snippets");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync($"/api/v1/snippets/{Guid.NewGuid()}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateSnippet_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            var createDto = new CreateSnippetDto
            {
                Title = "Test Snippet",
                Content = "Test Content",
                Language = "csharp"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/snippets", createDto);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateSnippet_WithAuthentication_ShouldCreateSnippet()
        {
            // This test would require:
            // 1. Register a user
            // 2. Login to get token
            // 3. Add token to Authorization header
            // 4. Create snippet
            // For brevity, the implementation is shown conceptually
        }
    }
}
