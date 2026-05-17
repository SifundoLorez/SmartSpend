using System;

namespace SmartSpend.ViewModels
{
    public class SavingsGoalViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal SavedAmount { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        // Calculated cleanly in controller — no dynamic issues
        public decimal Percentage { get; set; }
        public decimal Remaining { get; set; }
        public int DaysLeft { get; set; }
    }
}