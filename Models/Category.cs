using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSpend.Models
{
    public class Category
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public string Name { get; set; }

        public string Icon { get; set; } = "📦";

        public bool IsDefault { get; set; } = false;
    }
}