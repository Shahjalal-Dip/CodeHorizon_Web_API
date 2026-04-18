using System.Text.Json.Serialization;

namespace CodeHorizon.Application.DTOs
{
    public class ErrorResponseDto
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TraceId { get; set; }
    }
}