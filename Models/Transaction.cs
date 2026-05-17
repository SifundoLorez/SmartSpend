using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        public string Type { get; set; }  // "Income" or "Expense"

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsRecurring { get; set; } = false;

        public string Frequency { get; set; } // "Monthly", "Weekly"
    }
}