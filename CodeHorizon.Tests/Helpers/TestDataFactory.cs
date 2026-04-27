using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Text;
using CodeHorizon.Core.Entities;
using Bogus;

namespace CodeHorizon.Tests.Helpers
{
    public static class TestDataFactory
    {
        public static User CreateUser()
        {
            var faker = new Faker();
            return new User
            {
                Id = Guid.NewGuid(),
                Email = faker.Internet.Email(),
                Username = faker.Internet.UserName(),
                FullName = faker.Name.FullName(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                Bio = faker.Lorem.Sentence(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static Snippet CreateSnippet(User author, bool isPublic = true)
        {
            var faker = new Faker();
            return new Snippet
            {
                Id = Guid.NewGuid(),
                Title = faker.Lorem.Sentence(5),
                Content = faker.Lorem.Paragraphs(3),
                Description = faker.Lorem.Sentence(10),
                Language = faker.PickRandom("csharp", "javascript", "python", "java", "go"),
                IsPublic = isPublic,
                AuthorId = author.Id,
                Author = author,
                CreatedAt = DateTime.UtcNow,
                ViewCount = faker.Random.Int(0, 1000),
                BookmarkCount = faker.Random.Int(0, 100)
            };
        }

        public static List<Tag> CreateTags(List<string> tagNames)
        {
            var tags = new List<Tag>();
            foreach (var name in tagNames)
            {
                tags.Add(new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = name.ToLower()
                });
            }
            return tags;
        }
    }
}
