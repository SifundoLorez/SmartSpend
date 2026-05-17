using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;

namespace SmartSpend.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var user = db.Users.Find(userId);

            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.User = user;
            ViewBag.UserName = Session["UserName"];

            // Stats
            ViewBag.TotalTransactions = db.Transactions.Count(t => t.UserId == userId);
            ViewBag.TotalCategories = db.Categories.Count(c => c.UserId == userId);
            ViewBag.TotalGoals = db.SavingsGoals.Count(g => g.UserId == userId);
            ViewBag.TotalSubs = db.Subscriptions.Count(s => s.UserId == userId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateName(string firstName, string surname)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var user = db.Users.Find(userId);

            if (user != null && !string.IsNullOrWhiteSpace(firstName))
            {
                user.FirstName = firstName;
                user.Surname = surname;
                Session["UserName"] = firstName;
                db.SaveChanges();
                TempData["Success"] = "Name updated successfully.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(string currentPassword,
                                           string newPassword,
                                           string confirmPassword)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("Index");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("Index");
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Password must be at least 6 characters.";
                return RedirectToAction("Index");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            db.SaveChanges();
            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction("Index");
        }
    }
}