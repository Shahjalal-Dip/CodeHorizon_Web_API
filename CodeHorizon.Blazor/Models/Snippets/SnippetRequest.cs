namespace CodeHorizon.Blazor.Models.Snippets;

public class SnippetRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "csharp";
    public bool IsPublic { get; set; } = true;
    public List<string> Tags { get; set; } = [];
}
