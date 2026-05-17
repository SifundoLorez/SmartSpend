using System.ComponentModel.DataAnnotations;

namespace SmartSpend.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID must be 13 digits")]
        public string IdNumber { get; set; }

        public int Age { get; set; }
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}