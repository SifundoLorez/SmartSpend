using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartSpend.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsRecurring { get; set; } = false;

        public string Frequency { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}