using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public string ServiceName { get; set; }

        public string CustomName { get; set; }

        public string Icon { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime NextBillingDate { get; set; }

        public string Frequency { get; set; } = "Monthly";

        public bool IsActive { get; set; } = true;

        public bool WantToCancel { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Display name — custom overrides preset
        public string DisplayName => ServiceName == "Other"
            ? (CustomName ?? "Other")
            : ServiceName;
    }
}