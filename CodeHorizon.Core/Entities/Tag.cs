using System;
using System.Collections.Generic;

namespace CodeHorizon.Core.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation property
        public ICollection<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
    }
}