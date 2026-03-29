using System;

namespace CodeHorizon.Core.Entities
{
    public class SnippetTag
    {
        public Guid SnippetId { get; set; }
        public Guid TagId { get; set; }

        // Navigation properties
        public Snippet Snippet { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}