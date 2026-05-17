using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;

namespace SmartSpend.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var categories = db.Categories
                               .Where(c => c.UserId == userId)
                               .OrderBy(c => c.Name)
                               .ToList();

            ViewBag.UserName = Session["UserName"];
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string name, string icon)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(name))
            {
                db.Categories.Add(new Category
                {
                    UserId = GetUserId(),
                    Name = name,
                    Icon = string.IsNullOrWhiteSpace(icon) ? "📦" : icon,
                    IsDefault = false
                });
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string name, string icon)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var category = db.Categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (category != null)
            {
                category.Name = name;
                category.Icon = string.IsNullOrWhiteSpace(icon) ? "📦" : icon;
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
            var category = db.Categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (category != null)
            {
                // Check if category has transactions
                bool hasTransactions = db.Transactions.Any(t => t.CategoryId == id);
                if (hasTransactions)
                {
                    TempData["Error"] = "Cannot delete a category that has transactions.";
                    return RedirectToAction("Index");
                }

                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}