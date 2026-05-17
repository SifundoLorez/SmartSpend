using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SmartSpend.ViewModels
{
    public class SubscriptionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a service")]
        public string ServiceName { get; set; }

        public string CustomName { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Billing date is required")]
        public DateTime NextBillingDate { get; set; } = DateTime.Today.AddMonths(1);

        public string Frequency { get; set; } = "Monthly";

        public bool WantToCancel { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}