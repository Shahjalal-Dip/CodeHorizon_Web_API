namespace CodeHorizon.Blazor.Models.Common;

public class ApiError
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string? Message { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public string FriendlyMessage =>
        !string.IsNullOrWhiteSpace(Detail) ? Detail :
        !string.IsNullOrWhiteSpace(Message) ? Message :
        !string.IsNullOrWhiteSpace(Title) ? Title :
        "An unexpected error occurred.";
}
