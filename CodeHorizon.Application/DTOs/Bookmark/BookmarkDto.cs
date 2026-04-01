using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.DTOs.Bookmark
{
    public class BookmarkDto
    {
        public Guid Id { get; set; }
        public Guid SnippetId { get; set; }
        public string SnippetTitle { get; set; }=string.Empty;
        public string SnippetLanguage{ get; set; } = string.Empty; 
        public string SnippetAuthor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
