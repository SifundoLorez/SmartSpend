using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;
using SmartSpend.ViewModels;

namespace SmartSpend.Controllers
{
    public class SavingsGoalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var goals = db.SavingsGoals
                          .Where(g => g.UserId == userId)
                          .OrderBy(g => g.IsCompleted)
                          .ThenBy(g => g.TargetDate)
                          .ToList();

            // Map to ViewModel — calculate everything here cleanly
            var viewModels = goals.Select(g =>
            {
                decimal pct = g.TargetAmount > 0
                                    ? Math.Min(g.SavedAmount / g.TargetAmount * 100, 100)
                                    : 0;
                decimal remaining = g.TargetAmount - g.SavedAmount;
                int daysLeft = g.TargetDate.HasValue
                                    ? (g.TargetDate.Value - DateTime.Today).Days
                                    : -1;

                return new SavingsGoalViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    Icon = g.Icon ?? "🎯",
                    TargetAmount = g.TargetAmount,
                    SavedAmount = g.SavedAmount,
                    TargetDate = g.TargetDate,
                    IsCompleted = g.IsCompleted,
                    CreatedAt = g.CreatedAt,
                    Percentage = Math.Round(pct, 1),
                    Remaining = remaining < 0 ? 0 : remaining,
                    DaysLeft = daysLeft
                };
            }).ToList();

            decimal totalTarget = goals.Sum(g => g.TargetAmount);
            decimal totalSaved = goals.Sum(g => g.SavedAmount);

            ViewBag.Goals = viewModels;
            ViewBag.TotalTarget = totalTarget;
            ViewBag.TotalSaved = totalSaved;
            ViewBag.UserName = Session["UserName"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string name, string icon,
                                decimal targetAmount, DateTime? targetDate)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(name) && targetAmount > 0)
            {
                db.SavingsGoals.Add(new SavingsGoal
                {
                    UserId = GetUserId(),
                    Name = name,
                    Icon = string.IsNullOrWhiteSpace(icon) ? "🎯" : icon,
                    TargetAmount = targetAmount,
                    TargetDate = targetDate,
                    SavedAmount = 0,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                });

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFunds(int id, decimal amount)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var goal = db.SavingsGoals
                           .FirstOrDefault(g => g.Id == id && g.UserId == userId);

            if (goal != null && amount > 0)
            {
                goal.SavedAmount += amount;

                if (goal.SavedAmount >= goal.TargetAmount)
                {
                    goal.SavedAmount = goal.TargetAmount;
                    goal.IsCompleted = true;

                    db.Notifications.Add(new Notification
                    {
                        UserId = userId,
                        Title = "Goal Reached! 🎉",
                        Message = "Congratulations! You reached your savings goal: " + goal.Name,
                        Type = "success",
                        Icon = "🎉",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var goal = db.SavingsGoals
                           .FirstOrDefault(g => g.Id == id && g.UserId == userId);

            if (goal != null)
            {
                db.SavingsGoals.Remove(goal);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}