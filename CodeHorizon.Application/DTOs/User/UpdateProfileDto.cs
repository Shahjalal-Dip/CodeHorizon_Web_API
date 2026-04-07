using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CodeHorizon.Application.DTOs.User
{
    public class UpdateProfileDto
    {
        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        [MaxLength(200, ErrorMessage = "Profile picture URL cannot exceed 200 characters")]
        public string? ProfilePictureUrl { get; set; }

        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }
    }
}
