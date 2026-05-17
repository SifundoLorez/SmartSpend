using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartSpend.ViewModels
{
    public class BudgetSummaryViewModel
    {
        public int BudgetId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIcon { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal Spent { get; set; }
        public decimal Percent { get; set; }
        public decimal Remaining => LimitAmount - Spent;
    }
}