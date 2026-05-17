using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;

namespace SmartSpend.Controllers
{
    public class NotificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId() => (int)Session["UserId"];
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var notifications = db.Notifications
                                  .Where(n => n.UserId == userId)
                                  .OrderByDescending(n => n.CreatedAt)
                                  .ToList();

            ViewBag.Notifications = notifications;
            ViewBag.UnreadCount = notifications.Count(n => !n.IsRead);
            ViewBag.UserName = Session["UserName"];

            // Mark all as read
            foreach (var n in notifications.Where(n => !n.IsRead))
                n.IsRead = true;

            db.SaveChanges();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var notif = db.Notifications
                           .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notif != null)
            {
                db.Notifications.Remove(notif);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearAll()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();
            var all = db.Notifications.Where(n => n.UserId == userId).ToList();
            db.Notifications.RemoveRange(all);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Called via AJAX to get unread count for navbar badge
        public JsonResult UnreadCount()
        {
            if (Session["UserId"] == null)
                return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);

            int userId = (int)Session["UserId"];
            int count = db.Notifications
                           .Count(n => n.UserId == userId && !n.IsRead);

            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }
    }
}