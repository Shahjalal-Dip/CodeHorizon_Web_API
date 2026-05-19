namespace CodeHorizon.Blazor.Models.Snippets
{
    public class SnippetFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Language { get; set; }
        public string? Search { get; set; }
        public string? Tag { get; set; }
        public string SortBy { get; set; } = "created";
        public string SortOrder { get; set; } = "desc";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
