namespace CodeHorizon.Blazor.Models.Config;

public class ApiSettings
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = "https://localhost:7036";
    public string ApiVersionPath { get; set; } = "/api/v1";
    public string AuthPath { get; set; } = "/api/auth";
    public int RequestTimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;

    public string ApiV1Base => $"{BaseUrl.TrimEnd('/')}{ApiVersionPath}";
    public string AuthBase => $"{BaseUrl.TrimEnd('/')}{AuthPath}";
}

public class FeatureFlags
{
    public const string SectionName = "Features";

    public bool EnableAnalytics { get; set; } = true;
    public bool EnableHealthMonitor { get; set; } = true;
    public bool EnableOfflineDetection { get; set; } = true;
}
