using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;
using SmartSpend.ViewModels;

namespace SmartSpend.Controllers
{
    public class BudgetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            var budgets = db.Budgets
                            .Include("Category")
                            .Where(b => b.UserId == userId &&
                                        b.Month == month &&
                                        b.Year == year)
                            .ToList();

            var transactions = db.Transactions
                                 .Where(t => t.UserId == userId &&
                                             t.Date.Month == month &&
                                             t.Date.Year == year &&
                                             t.Type == "Expense")
                                 .ToList();

            var summary = budgets.Select(b =>
            {
                decimal spent = transactions
                    .Where(t => t.CategoryId == b.CategoryId)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal percent = b.LimitAmount > 0
                    ? spent / b.LimitAmount * 100
                    : 0;

                return new BudgetSummaryViewModel
                {
                    BudgetId = b.Id,
                    CategoryName = b.Category?.Name ?? "Uncategorized",
                    CategoryIcon = b.Category?.Icon ?? "📦",
                    LimitAmount = b.LimitAmount,
                    Spent = spent,
                    Percent = percent
                };
            }).ToList();

            ViewBag.Summary = summary;
            ViewBag.Categories = db.Categories
                                   .Where(c => c.UserId == userId)
                                   .ToList();
            ViewBag.UserName = Session["UserName"];
            ViewBag.MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int categoryId, decimal limitAmount)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            var existing = db.Budgets.FirstOrDefault(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year);

            if (existing != null)
                existing.LimitAmount = limitAmount;
            else
                db.Budgets.Add(new Budget
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    LimitAmount = limitAmount,
                    Month = month,
                    Year = year
                });

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            int userId = GetUserId(); // ✅ FIX

            var budget = db.Budgets
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (budget != null)
            {
                db.Budgets.Remove(budget);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}