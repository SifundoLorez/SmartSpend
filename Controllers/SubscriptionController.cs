using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;
using SmartSpend.ViewModels;

namespace SmartSpend.Controllers
{
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var subscriptions = db.Subscriptions
                                  .Where(s => s.UserId == userId)
                                  .OrderBy(s => s.NextBillingDate)
                                  .ToList();

            // Auto-renew past billing dates
            foreach (var s in subscriptions.Where(s => s.IsActive))
            {
                while (s.NextBillingDate < DateTime.Today)
                {
                    s.NextBillingDate = s.Frequency == "Monthly"
                        ? s.NextBillingDate.AddMonths(1)
                        : s.NextBillingDate.AddYears(1);
                }
            }

            db.SaveChanges();

            decimal totalMonthly = subscriptions
                .Where(s => s.IsActive && s.Frequency == "Monthly")
                .Sum(s => (decimal?)s.Amount) ?? 0;

            decimal totalYearly = subscriptions
                .Where(s => s.IsActive)
                .Sum(s => s.Frequency == "Monthly"
                    ? s.Amount * 12
                    : s.Amount);

            ViewBag.Subscriptions = subscriptions;
            ViewBag.TotalMonthly = totalMonthly;
            ViewBag.TotalYearly = totalYearly;
            ViewBag.UserName = Session["UserName"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SubscriptionViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var icons = GetServiceIcons();

                db.Subscriptions.Add(new Subscription
                {
                    UserId = GetUserId(),
                    ServiceName = model.ServiceName,
                    CustomName = model.ServiceName == "Other" ? model.CustomName : null,
                    Icon = icons.ContainsKey(model.ServiceName)
                                        ? icons[model.ServiceName]
                                        : "🔔",
                    Amount = model.Amount,
                    NextBillingDate = model.NextBillingDate,
                    Frequency = model.Frequency,
                    IsActive = true,
                    WantToCancel = false
                });

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleCancel(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId(); // ✅ store first
            var sub = db.Subscriptions
                        .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (sub != null)
            {
                sub.WantToCancel = !sub.WantToCancel;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId(); // ✅ store first
            var sub = db.Subscriptions
                        .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (sub != null)
            {
                sub.IsActive = !sub.IsActive;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId(); // ✅ store first
            var sub = db.Subscriptions
                        .FirstOrDefault(s => s.Id == id && s.UserId == userId);

            if (sub != null)
            {
                db.Subscriptions.Remove(sub);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private System.Collections.Generic.Dictionary<string, string> GetServiceIcons()
        {
            return new System.Collections.Generic.Dictionary<string, string>
            {
                { "Spotify",      "🎵" },
                { "YouTube",      "▶️" },
                { "Netflix",      "🎬" },
                { "Apple Music",  "🍎" },
                { "Showmax",      "📺" },
                { "Disney+",      "✨" },
                { "DStv",         "📡" },
                { "Other",        "🔔" }
            };
        }
    }
}