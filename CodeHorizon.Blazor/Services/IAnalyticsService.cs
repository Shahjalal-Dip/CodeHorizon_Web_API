namespace CodeHorizon.Blazor.Services;

public interface IAnalyticsService
{
    void TrackPageView(string path);
    IReadOnlyList<PageViewEntry> GetRecentViews();
}

public record PageViewEntry(string Path, DateTime Timestamp);
