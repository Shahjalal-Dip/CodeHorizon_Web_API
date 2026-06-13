using CodeHorizon.Blazor.Models.Snippets;
using CodeHorizon.Blazor.Models.Tags;
using CodeHorizon.Blazor.Models.Users;

namespace CodeHorizon.Blazor.Services;

public class StateContainer
{
    public List<SnippetResponse>? CachedPublicSnippets { get; set; }
    public List<TagResponse>? CachedTags { get; set; }
    public UserProfileResponse? CachedProfile { get; set; }
    public DateTime? SnippetsCacheTime { get; set; }
    public DateTime? TagsCacheTime { get; set; }
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    public bool IsSnippetsCacheValid =>
        CachedPublicSnippets is not null &&
        SnippetsCacheTime.HasValue &&
        DateTime.UtcNow - SnippetsCacheTime < CacheDuration;

    public bool IsTagsCacheValid =>
        CachedTags is not null &&
        TagsCacheTime.HasValue &&
        DateTime.UtcNow - TagsCacheTime < CacheDuration;

    public void InvalidateSnippets()
    {
        CachedPublicSnippets = null;
        SnippetsCacheTime = null;
    }

    public void InvalidateTags()
    {
        CachedTags = null;
        TagsCacheTime = null;
    }

    public void InvalidateProfile() => CachedProfile = null;
}
