namespace CodeHorizon.Blazor.Models.Users
{
    public class UserProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int SnippetsCount { get; set; }
        public int BookmarksCount { get; set; }
    }
}
