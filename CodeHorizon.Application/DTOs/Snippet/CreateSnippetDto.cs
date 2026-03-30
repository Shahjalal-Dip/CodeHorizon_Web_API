using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CodeHorizon.Application.DTOs.Snippet
{
    public class CreateSnippetDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language is required")]
        public string Language { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = true;

        public List<string> Tags { get; set; } = new List<string>();
    }
}