using System;
using System.Collections.Generic;

namespace CodeHorizon.Core.Entities
{
    public class Snippet
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string? CodePreview { get; set; }
        public int ViewCount { get; set; } = 0;
        public int BookmarkCount { get; set; } = 0;
        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid AuthorId { get; set; }

        // Navigation properties
        public User Author { get; set; } = null!;
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
    }
}