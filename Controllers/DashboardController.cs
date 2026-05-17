using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;

namespace SmartSpend.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            var transactions = db.Transactions
                                 .Include("Category")
                                 .Where(t => t.UserId == userId)
                                 .ToList();

            var thisMonth = transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();

            var lastMonth = transactions
                .Where(t => t.Date.Month == (month == 1 ? 12 : month - 1)
                         && t.Date.Year == (month == 1 ? year - 1 : year))
                .ToList();

            // ── Summary ──
            decimal totalIncome = thisMonth.Where(t => t.Type == "Income").Sum(t => (decimal?)t.Amount) ?? 0;
            decimal totalExpenses = thisMonth.Where(t => t.Type == "Expense").Sum(t => (decimal?)t.Amount) ?? 0;
            decimal balance = totalIncome - totalExpenses;

            // ── Last month comparison ──
            decimal lastExpenses = lastMonth.Where(t => t.Type == "Expense").Sum(t => (decimal?)t.Amount) ?? 0;
            decimal lastIncome = lastMonth.Where(t => t.Type == "Income").Sum(t => (decimal?)t.Amount) ?? 0;

            // ── Spending by category (for pie chart) ──
            var byCategory = thisMonth
                .Where(t => t.Type == "Expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new {
                    Name = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            // ── Monthly trend last 6 months (for bar chart) ──
            var trend = Enumerable.Range(0, 6).Select(i => {
                var d = DateTime.Now.AddMonths(-i);
                var income = transactions
                    .Where(t => t.Date.Month == d.Month && t.Date.Year == d.Year && t.Type == "Income")
                    .Sum(t => (decimal?)t.Amount) ?? 0;
                var expense = transactions
                    .Where(t => t.Date.Month == d.Month && t.Date.Year == d.Year && t.Type == "Expense")
                    .Sum(t => (decimal?)t.Amount) ?? 0;
                return new
                {
                    Month = d.ToString("MMM"),
                    Income = income,
                    Expense = expense
                };
            }).Reverse().ToList();

            // ── Smart Insights ──
            var insights = new System.Collections.Generic.List<string>();

            if (lastExpenses > 0 && totalExpenses > 0)
            {
                decimal diff = ((totalExpenses - lastExpenses) / lastExpenses) * 100;
                if (diff > 0)
                    insights.Add($"⚠️ You spent {Math.Abs(diff):0}% more this month than last month.");
                else
                    insights.Add($"✅ You spent {Math.Abs(diff):0}% less this month than last month.");
            }

            if (byCategory.Any())
                insights.Add($"🔥 Your highest expense is <b>{byCategory.First().Name}</b> at R{byCategory.First().Amount:N2}.");

            if (balance > 0)
                insights.Add($"💰 You saved <b>R{balance:N2}</b> this month.");
            else if (balance < 0)
                insights.Add($"🚨 You are over budget by <b>R{Math.Abs(balance):N2}</b> this month.");

            if (lastIncome > 0 && totalIncome > lastIncome)
                insights.Add($"📈 Your income increased by R{(totalIncome - lastIncome):N2} compared to last month.");

            // ── Budgets warning ──
            var budgets = db.Budgets
                            .Include("Category")
                            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                            .ToList();

            foreach (var budget in budgets)
            {
                var spent = thisMonth
                    .Where(t => t.Type == "Expense" && t.CategoryId == budget.CategoryId)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal pct = budget.LimitAmount > 0 ? (spent / budget.LimitAmount) * 100 : 0;

                if (pct >= 100)
                    insights.Add($"🚨 <b>{budget.Category.Name}</b> budget exceeded! Spent R{spent:N2} of R{budget.LimitAmount:N2}.");
                else if (pct >= 80)
                    insights.Add($"⚠️ <b>{budget.Category.Name}</b> budget is at {pct:0}%. R{(budget.LimitAmount - spent):N2} remaining.");
            }

            // ── Recent transactions ──
            var recent = transactions
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            // ── Pass to view ──
            ViewBag.UserName = Session["UserName"];
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.Balance = balance;
            ViewBag.Insights = insights;
            ViewBag.Recent = recent;

            // JSON for charts
            ViewBag.ChartLabels = Newtonsoft.Json.JsonConvert.SerializeObject(byCategory.Select(x => x.Name).ToList());
            ViewBag.ChartData = Newtonsoft.Json.JsonConvert.SerializeObject(byCategory.Select(x => x.Amount).ToList());
            ViewBag.TrendMonths = Newtonsoft.Json.JsonConvert.SerializeObject(trend.Select(x => x.Month).ToList());
            ViewBag.TrendIncome = Newtonsoft.Json.JsonConvert.SerializeObject(trend.Select(x => x.Income).ToList());
            ViewBag.TrendExpense = Newtonsoft.Json.JsonConvert.SerializeObject(trend.Select(x => x.Expense).ToList());

            // Upcoming subscriptions (next 3 by billing date)
            var upcomingSubs = db.Subscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderBy(s => s.NextBillingDate)
                .Take(4)
                .ToList();

            ViewBag.UpcomingSubs = upcomingSubs;

            return View();
        }
    }
}