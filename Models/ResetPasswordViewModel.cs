using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SmartSpend.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}