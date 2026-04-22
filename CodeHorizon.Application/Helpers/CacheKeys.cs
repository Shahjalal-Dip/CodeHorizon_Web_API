using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.Helpers
{
    public static class CacheKeys
    {
        public static string SnippetKey(Guid id) => $"snippet_{id}";
        public static string SnippetsListKey(int page, int pageSize, string? language, string? search)
            => $"snippets_{page}_{pageSize}_{language ?? "all"}_{search ?? "all"}";
        public static string PopularTagsKey(int count) => $"popular_tags_{count}";
        public static string UserProfileKey(Guid userId) => $"user_profile_{userId}";
        public static string AllTagsKey => "all_tags";

        public static string SnippetBookmarkStatusKey(Guid userId, Guid snippetId)
            => $"bookmark_status_{userId}_{snippetId}";
    }
}
