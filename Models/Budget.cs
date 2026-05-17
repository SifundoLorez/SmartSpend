using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        public decimal LimitAmount { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
    }
}