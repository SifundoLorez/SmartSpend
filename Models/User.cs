using System;
using System.ComponentModel.DataAnnotations;

namespace SmartSpend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13)]
        public string IdNumber { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}