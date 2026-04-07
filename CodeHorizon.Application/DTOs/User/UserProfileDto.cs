using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeHorizon.Application.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get;set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public String ProfilePictureUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int SnippetsCount { get; set; }
        public int BookmarksCount { get; set; }

    }
}
