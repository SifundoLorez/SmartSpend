using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public string Type { get; set; } = "info";
        // "info", "warning", "danger", "success"

        public string Icon { get; set; } = "🔔";

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}