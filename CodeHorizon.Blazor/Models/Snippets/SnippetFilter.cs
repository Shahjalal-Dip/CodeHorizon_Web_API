namespace CodeHorizon.Blazor.Models.Snippets;

public class SnippetFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? Language { get; set; }
    public string? Search { get; set; }
    public string? Tag { get; set; }
    public string SortBy { get; set; } = "created";
    public string SortOrder { get; set; } = "desc";
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string ToQueryString()
    {
        var parts = new List<string>
        {
            $"page={Page}",
            $"pageSize={PageSize}",
            $"sortBy={Uri.EscapeDataString(SortBy)}",
            $"sortOrder={Uri.EscapeDataString(SortOrder)}"
        };

        if (!string.IsNullOrWhiteSpace(Language))
            parts.Add($"language={Uri.EscapeDataString(Language)}");
        if (!string.IsNullOrWhiteSpace(Search))
            parts.Add($"search={Uri.EscapeDataString(Search)}");
        if (!string.IsNullOrWhiteSpace(Tag))
            parts.Add($"tag={Uri.EscapeDataString(Tag)}");
        if (FromDate.HasValue)
            parts.Add($"fromDate={FromDate.Value:O}");
        if (ToDate.HasValue)
            parts.Add($"toDate={ToDate.Value:O}");

        return string.Join("&", parts);
    }
}
