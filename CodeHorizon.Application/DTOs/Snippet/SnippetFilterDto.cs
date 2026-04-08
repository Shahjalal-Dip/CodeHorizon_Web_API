using CodeHorizon.Core.Entities;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.DTOs.Snippet
{
    public class SnippetFilterDto
    {
        public string? Language { get; set; }
        public string? Search { get; set; }
        public string? Tag { get; set; }
        public string? SortBy { get; set; } // "created", "views", "bookmarks"
        public string? SortOrder { get; set; } // "asc", "desc"
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsPublic { get; set; }
        public Guid? AuthorId { get; set; }

    }
}
