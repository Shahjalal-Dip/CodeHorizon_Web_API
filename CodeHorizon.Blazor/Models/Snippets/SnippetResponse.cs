namespace CodeHorizon.Blazor.Models.Snippets;

public class SnippetResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string CodePreview { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int BookmarkCount { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorFullName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public bool IsBookmarkedByCurrentUser { get; set; }
}
