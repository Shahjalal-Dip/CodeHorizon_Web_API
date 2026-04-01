using System;

namespace CodeHorizon.Application.DTOs.Bookmark
{
    public class BookmarkResponseDto
    {
        public Guid Id { get; set; }
        public Guid SnippetId { get; set; }
        public string SnippetTitle { get; set; } = string.Empty;
        public string SnippetLanguage { get; set; } = string.Empty;
        public string AuthorUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}