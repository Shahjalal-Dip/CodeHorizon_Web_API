using System;

namespace CodeHorizon.Core.Entities
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SnippetId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Snippet Snippet { get; set; } = null!;
    }
}