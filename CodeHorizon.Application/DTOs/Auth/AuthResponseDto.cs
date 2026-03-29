using System;
using System.Collections.Generic;
using System.Text;

namespace CodeHorizon.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
