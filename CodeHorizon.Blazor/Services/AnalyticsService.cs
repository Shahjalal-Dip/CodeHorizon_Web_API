namespace CodeHorizon.Blazor.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly List<PageViewEntry> _views = [];
    private const int MaxEntries = 100;

    public void TrackPageView(string path)
    {
        _views.Insert(0, new PageViewEntry(path, DateTime.UtcNow));
        if (_views.Count > MaxEntries)
            _views.RemoveAt(_views.Count - 1);
    }

    public IReadOnlyList<PageViewEntry> GetRecentViews() => _views.AsReadOnly();
}
