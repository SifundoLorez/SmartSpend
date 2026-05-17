using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.ViewModels;

namespace SmartSpend.Controllers
{
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index(int month = 0, int year = 0)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var transactions = db.Transactions
                                 .Include("Category")
                                 .Where(t => t.UserId == userId &&
                                             t.Date.Month == month &&
                                             t.Date.Year == year)
                                 .OrderByDescending(t => t.Date)
                                 .ToList();

            decimal totalIncome = transactions
                .Where(t => t.Type == "Income")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            decimal totalExpenses = transactions
                .Where(t => t.Type == "Expense")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            decimal balance = totalIncome - totalExpenses;

            // Strongly typed category breakdown
            var byCategory = transactions
                .Where(t => t.Type == "Expense" && t.Category != null)
                .GroupBy(t => new { t.Category.Name, t.Category.Icon })
                .Select(g => new CategoryReportViewModel
                {
                    Name = g.Key.Name ?? "Uncategorized",
                    Icon = g.Key.Icon ?? "📦",
                    Amount = g.Sum(t => t.Amount),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            ViewBag.Transactions = transactions;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.Balance = balance;
            ViewBag.ByCategory = byCategory;
            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy");
            ViewBag.UserName = Session["UserName"];

            return View();
        }

        public ActionResult ExportCsv(int month = 0, int year = 0)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var transactions = db.Transactions
                                 .Include("Category")
                                 .Where(t => t.UserId == userId &&
                                             t.Date.Month == month &&
                                             t.Date.Year == year)
                                 .OrderByDescending(t => t.Date)
                                 .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Date,Type,Category,Description,Amount");

            foreach (var t in transactions)
            {
                sb.AppendLine(
                    $"{t.Date:yyyy-MM-dd}," +
                    $"{t.Type}," +
                    $"{t.Category?.Name ?? "Uncategorized"}," +
                    $"\"{t.Description ?? ""}\"," +
                    $"{t.Amount}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"SmartSpend_{month}_{year}.csv";
            return File(bytes, "text/csv", fileName);
        }
    }
}