namespace CodeHorizon.Blazor.Models.Bookmarks;

public class BookmarkResponse
{
    public Guid Id { get; set; }
    public Guid SnippetId { get; set; }
    public string SnippetTitle { get; set; } = string.Empty;
    public string SnippetLanguage { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
