using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartSpend.ViewModels
{
    public class CategoryReportViewModel
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}