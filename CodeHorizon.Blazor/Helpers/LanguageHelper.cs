namespace CodeHorizon.Blazor.Helpers;

public static class LanguageHelper
{
    public static readonly IReadOnlyList<string> SupportedLanguages =
    [
        "csharp", "javascript", "typescript", "python", "java", "go", "rust",
        "cpp", "c", "sql", "html", "css", "json", "xml", "bash", "powershell", "php", "ruby", "swift", "kotlin"
    ];

    public static string ToPrismClass(string language) =>
        language.ToLowerInvariant() switch
        {
            "csharp" or "cs" => "language-csharp",
            "javascript" or "js" => "language-javascript",
            "typescript" or "ts" => "language-typescript",
            _ => $"language-{language.ToLowerInvariant()}"
        };
}
