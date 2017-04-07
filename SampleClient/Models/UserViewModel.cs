using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SampleClient.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Given Name is required")]
        public string GivenName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime PasswordResetTokenExpiresAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool SendEmail { get; set; }
    }
}