using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class SavingsGoal
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public string Name { get; set; }

        public string Icon { get; set; } = "🎯";

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal SavedAmount { get; set; } = 0;

        public DateTime? TargetDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal Remaining => TargetAmount - SavedAmount;
        public decimal Percentage => TargetAmount > 0
            ? Math.Min(SavedAmount / TargetAmount * 100, 100)
            : 0;
    }
}