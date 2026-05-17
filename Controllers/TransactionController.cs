using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;
using SmartSpend.ViewModels;

namespace SmartSpend.Controllers
{
    public class TransactionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int GetUserId()
        {
            return (int)Session["UserId"];
        }

        private bool IsLoggedIn()
        {
            return Session["UserId"] != null;
        }

        // ── List all transactions ──
        public ActionResult Index(string type = "", int categoryId = 0,
                                  string search = "", string from = "", string to = "")
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var query = db.Transactions
                          .Include("Category")
                          .Where(t => t.UserId == userId);

            // Filters
            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Type == type);

            if (categoryId > 0)
                query = query.Where(t => t.CategoryId == categoryId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Description.Contains(search));

            if (!string.IsNullOrEmpty(from) && DateTime.TryParse(from, out DateTime fromDate))
                query = query.Where(t => t.Date >= fromDate);

            if (!string.IsNullOrEmpty(to) && DateTime.TryParse(to, out DateTime toDate))
                query = query.Where(t => t.Date <= toDate.AddDays(1));

            var transactions = query.OrderByDescending(t => t.Date).ToList();

            ViewBag.Categories = db.Categories.Where(c => c.UserId == userId).ToList();
            ViewBag.SelectedType = type;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;
            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.UserName = Session["UserName"];

            return View(transactions);
        }

        // ── Add GET ──
        public ActionResult Add()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var model = new TransactionViewModel
            {
                Date = DateTime.Now,
                Categories = db.Categories
                               .Where(c => c.UserId == userId)
                               .Select(c => new SelectListItem
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.Icon + " " + c.Name
                               }).ToList()
            };

            ViewBag.UserName = Session["UserName"];
            return View(model);
        }

        // ── Add POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(TransactionViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    UserId = userId,
                    CategoryId = model.CategoryId,
                    Type = model.Type,
                    Amount = model.Amount,
                    Description = model.Description,
                    Date = model.Date,
                    IsRecurring = model.IsRecurring,
                    Frequency = model.IsRecurring ? model.Frequency : null
                };

                db.Transactions.Add(transaction);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            model.Categories = db.Categories
                                  .Where(c => c.UserId == userId)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.Icon + " " + c.Name
                                  }).ToList();

            ViewBag.UserName = Session["UserName"];
            return View(model);
        }

        // ── Edit GET ──
        public ActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var transaction = db.Transactions
                                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (transaction == null) return HttpNotFound();

            var model = new TransactionViewModel
            {
                Id = transaction.Id,
                CategoryId = transaction.CategoryId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                IsRecurring = transaction.IsRecurring,
                Frequency = transaction.Frequency,
                Categories = db.Categories
                                .Where(c => c.UserId == userId)
                                .Select(c => new SelectListItem
                                {
                                    Value = c.Id.ToString(),
                                    Text = c.Icon + " " + c.Name
                                }).ToList()
            };

            ViewBag.UserName = Session["UserName"];
            return View(model);
        }

        // ── Edit POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TransactionViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            if (ModelState.IsValid)
            {
                var transaction = db.Transactions
                                    .FirstOrDefault(t => t.Id == model.Id && t.UserId == userId);

                if (transaction == null) return HttpNotFound();

                transaction.CategoryId = model.CategoryId;
                transaction.Type = model.Type;
                transaction.Amount = model.Amount;
                transaction.Description = model.Description;
                transaction.Date = model.Date;
                transaction.IsRecurring = model.IsRecurring;
                transaction.Frequency = model.IsRecurring ? model.Frequency : null;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            model.Categories = db.Categories
                                  .Where(c => c.UserId == userId)
                                  .Select(c => new SelectListItem
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.Icon + " " + c.Name
                                  }).ToList();

            ViewBag.UserName = Session["UserName"];
            return View(model);
        }

        // ── Delete ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = GetUserId();

            var transaction = db.Transactions
                                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (transaction != null)
            {
                db.Transactions.Remove(transaction);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}