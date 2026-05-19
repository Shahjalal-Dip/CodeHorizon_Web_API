namespace CodeHorizon.Blazor.Models.Common
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}
