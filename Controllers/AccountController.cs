using System;
using System.Linq;
using System.Web.Mvc;
using SmartSpend.Data;
using SmartSpend.Models;
using SmartSpend.ViewModels;
using SmartSpend.Helpers;

namespace SmartSpend.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ── Register GET ──
        public ActionResult Register()
        {
            return View();
        }

        // ── Register POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "An account with this email already exists.");
                    return View(model);
                }

                var (age, gender) = ExtractIdInfo(model.IdNumber);

                var user = new User
                {
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    IdNumber = model.IdNumber,
                    Age = age,
                    Gender = gender,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CreatedAt = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();

                SeedDefaultCategories(user.Id);
                EmailService.SendWelcomeEmail(user.Email, user.FirstName);

                // ✅ Pass name to success page
                TempData["RegisteredName"] = user.FirstName;
                return RedirectToAction("RegisterSuccess");
            }

            return View(model);
        }

        // ✅ New action
        public ActionResult RegisterSuccess()
        {
            // If someone navigates here directly without registering, send them away
            if (TempData["RegisteredName"] == null)
                return RedirectToAction("Register");

            ViewBag.RegisteredName = TempData["RegisteredName"];
            ViewBag.HideSidebar = true;
            return View();
        }

        // ── Login GET ──
        public ActionResult Login()
        {
            return View();
        }

        // ── Login POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                Session["UserId"] = user.Id;
                Session["UserName"] = user.FirstName;
                //ProcessRecurringTransactions(user.Id);
                // After ProcessRecurringTransactions(user.Id);
                GenerateNotifications(user.Id);

                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }

        // ── Logout ──
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ── SA ID extraction ──
        private (int age, string gender) ExtractIdInfo(string idNumber)
        {
            int year = int.Parse(idNumber.Substring(0, 2));
            int month = int.Parse(idNumber.Substring(2, 2));
            int day = int.Parse(idNumber.Substring(4, 2));

            int fullYear = year > 25 ? 1900 + year : 2000 + year;
            DateTime dob = new DateTime(fullYear, month, day);

            int age = DateTime.Now.Year - dob.Year;
            if (DateTime.Now < dob.AddYears(age)) age--;

            int genderDigits = int.Parse(idNumber.Substring(6, 4));
            string gender = genderDigits >= 5000 ? "Male" : "Female";

            return (age, gender);
        }

        private void SeedDefaultCategories(int userId)
        {
            var defaults = new[]
            {
        new Category { UserId = userId, Name = "Food & Groceries", Icon = "🍔", IsDefault = true },
        new Category { UserId = userId, Name = "Transport",        Icon = "🚗", IsDefault = true },
        new Category { UserId = userId, Name = "Rent & Housing",   Icon = "🏠", IsDefault = true },
        new Category { UserId = userId, Name = "Entertainment",    Icon = "🎬", IsDefault = true },
        new Category { UserId = userId, Name = "Healthcare",       Icon = "💊", IsDefault = true },
        new Category { UserId = userId, Name = "Education",        Icon = "📚", IsDefault = true },
        new Category { UserId = userId, Name = "Shopping",         Icon = "🛍️", IsDefault = true },
        new Category { UserId = userId, Name = "Salary",           Icon = "💼", IsDefault = true },
        new Category { UserId = userId, Name = "Other",            Icon = "📦", IsDefault = true },
    };

            db.Categories.AddRange(defaults);
            db.SaveChanges();
        }

        private void GenerateNotifications(int userId)
        {
            var today = DateTime.Today;
            int month = today.Month;
            int year = today.Year;

            // ── 1. Budget warnings ──
            var budgets = db.Budgets
                            .Where(b => b.UserId == userId &&
                                        b.Month == month &&
                                        b.Year == year)
                            .ToList();

            foreach (var budget in budgets)
            {
                // Load category separately to avoid Include issues
                var category = db.Categories.Find(budget.CategoryId);
                if (category == null) continue;

                decimal spent = db.Transactions
                    .Where(t => t.UserId == userId &&
                                t.CategoryId == budget.CategoryId &&
                                t.Type == "Expense" &&
                                t.Date.Month == month &&
                                t.Date.Year == year)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                decimal pct = budget.LimitAmount > 0
                    ? spent / budget.LimitAmount * 100
                    : 0;

                if (pct >= 100)
                {
                    // Only add once per day
                    bool exists = db.Notifications.Any(n =>
                        n.UserId == userId &&
                        n.Type == "danger" &&
                        n.Title.Contains(category.Name) &&
                        n.Title.Contains("Exceeded") &&
                        n.CreatedAt >= today);

                    if (!exists)
                    {
                        db.Notifications.Add(new Models.Notification
                        {
                            UserId = userId,
                            Title = "Budget Exceeded: " + category.Name,
                            Message = "You have exceeded your " + category.Name +
                                        " budget. Spent R" + spent.ToString("N2") +
                                        " of R" + budget.LimitAmount.ToString("N2") + ".",
                            Type = "danger",
                            Icon = "🚨",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
                else if (pct >= 80)
                {
                    bool exists = db.Notifications.Any(n =>
                        n.UserId == userId &&
                        n.Type == "warning" &&
                        n.Title.Contains(category.Name) &&
                        n.Title.Contains("Warning") &&
                        n.CreatedAt >= today);

                    if (!exists)
                    {
                        db.Notifications.Add(new Models.Notification
                        {
                            UserId = userId,
                            Title = "Budget Warning: " + category.Name,
                            Message = "You are at " + pct.ToString("0") + "% of your " +
                                        category.Name + " budget. R" +
                                        (budget.LimitAmount - spent).ToString("N2") + " remaining.",
                            Type = "warning",
                            Icon = "⚠️",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            // ── 2. Subscription reminders ──
            var subs = db.Subscriptions
                         .Where(s => s.UserId == userId && s.IsActive)
                         .ToList();

            foreach (var sub in subs)
            {
                // Auto-advance past billing dates
                while (sub.NextBillingDate < today)
                {
                    sub.NextBillingDate = sub.Frequency == "Monthly"
                        ? sub.NextBillingDate.AddMonths(1)
                        : sub.NextBillingDate.AddYears(1);
                }

                int days = (sub.NextBillingDate - today).Days;

                // Billing reminder — due within 3 days
                if (days <= 3 && days >= 0)
                {
                    string subName = sub.ServiceName == "Other"
                        ? (sub.CustomName ?? "Other")
                        : sub.ServiceName;

                    bool exists = db.Notifications.Any(n =>
                        n.UserId == userId &&
                        n.Title.Contains(subName) &&
                        n.Title.Contains("Due") &&
                        n.CreatedAt >= today);

                    if (!exists)
                    {
                        string when = days == 0 ? "today"
                                    : days == 1 ? "tomorrow"
                                    : "in " + days + " days";

                        db.Notifications.Add(new Models.Notification
                        {
                            UserId = userId,
                            Title = "Subscription Due: " + subName,
                            Message = "Your " + subName + " subscription of R" +
                                        sub.Amount.ToString("N2") + " is due " + when +
                                        " (" + sub.NextBillingDate.ToString("dd MMM yyyy") + ").",
                            Type = days == 0 ? "danger" : "warning",
                            Icon = sub.Icon ?? "🔔",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }

                // Cancel reminder — flagged and due within 5 days
                if (sub.WantToCancel && days <= 5 && days >= 0)
                {
                    string subName = sub.ServiceName == "Other"
                        ? (sub.CustomName ?? "Other")
                        : sub.ServiceName;

                    bool exists = db.Notifications.Any(n =>
                        n.UserId == userId &&
                        n.Title.Contains(subName) &&
                        n.Title.Contains("Cancel") &&
                        n.CreatedAt >= today);

                    if (!exists)
                    {
                        db.Notifications.Add(new Models.Notification
                        {
                            UserId = userId,
                            Title = "Cancel Reminder: " + subName,
                            Message = "You flagged " + subName +
                                        " for cancellation. Billing date is " +
                                        sub.NextBillingDate.ToString("dd MMM yyyy") +
                                        ". Cancel now to avoid being charged.",
                            Type = "danger",
                            Icon = "❌",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            db.SaveChanges(); // Save subscription date updates

            // ── 3. Savings goal deadline reminders ──
            var goals = db.SavingsGoals
                          .Where(g => g.UserId == userId && !g.IsCompleted)
                          .ToList();

            foreach (var goal in goals)
            {
                if (!goal.TargetDate.HasValue) continue;

                int daysToGoal = (goal.TargetDate.Value - today).Days;

                if (daysToGoal <= 7 && daysToGoal >= 0 &&
                    goal.SavedAmount < goal.TargetAmount)
                {
                    bool exists = db.Notifications.Any(n =>
                        n.UserId == userId &&
                        n.Title.Contains(goal.Name) &&
                        n.Title.Contains("Goal") &&
                        n.CreatedAt >= today);

                    if (!exists)
                    {
                        db.Notifications.Add(new Models.Notification
                        {
                            UserId = userId,
                            Title = "Goal Deadline: " + goal.Name,
                            Message = "Your savings goal '" + goal.Name +
                                        "' is due in " + daysToGoal + " days. " +
                                        "You still need R" + goal.Remaining.ToString("N2") +
                                        " to reach your target of R" +
                                        goal.TargetAmount.ToString("N2") + ".",
                            Type = "warning",
                            Icon = "🎯",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }

            // ── 4. General welcome/tip if no notifications exist yet ──
            bool hasAny = db.Notifications.Any(n => n.UserId == userId);
            if (!hasAny)
            {
                db.Notifications.Add(new Models.Notification
                {
                    UserId = userId,
                    Title = "Welcome to SmartSpend! 👋",
                    Message = "Start by adding transactions, setting budgets, and tracking your subscriptions to get smart notifications here.",
                    Type = "info",
                    Icon = "💡",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
            }

            db.SaveChanges();
        }

        //private void ProcessRecurringTransactions(int userId)
        //{
        //    var today = DateTime.Today;

        //    var recurring = db.Transactions
        //                      .Where(t => t.UserId == userId && t.IsRecurring)
        //                      .ToList();

        //    foreach (var t in recurring)
        //    {
        //        if (t.Frequency == "Monthly")
        //        {
        //            // Already processed this month?
        //            bool exists = db.Transactions.Any(x =>
        //                x.UserId == userId &&
        //                x.CategoryId == t.CategoryId &&
        //                x.Amount == t.Amount &&
        //                x.Type == t.Type &&
        //                x.Date.Month == today.Month &&
        //                x.Date.Year == today.Year &&
        //                x.IsRecurring == false);

        //            if (!exists)
        //            {
        //                db.Transactions.Add(new Models.Transaction
        //                {
        //                    UserId = userId,
        //                    CategoryId = t.CategoryId,
        //                    Type = t.Type,
        //                    Amount = t.Amount,
        //                    Description = (t.Description ?? "") + " (Auto)",
        //                    Date = today,
        //                    IsRecurring = false
        //                });
        //            }
        //        }
        //        else if (t.Frequency == "Weekly")
        //        {
        //            // Already processed this week?
        //            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

        //            bool exists = db.Transactions.Any(x =>
        //                x.UserId == userId &&
        //                x.CategoryId == t.CategoryId &&
        //                x.Amount == t.Amount &&
        //                x.Type == t.Type &&
        //                x.Date >= startOfWeek &&
        //                x.Date <= today &&
        //                x.IsRecurring == false);

        //            if (!exists)
        //            {
        //                db.Transactions.Add(new Models.Transaction
        //                {
        //                    UserId = userId,
        //                    CategoryId = t.CategoryId,
        //                    Type = t.Type,
        //                    Amount = t.Amount,
        //                    Description = (t.Description ?? "") + " (Auto)",
        //                    Date = today,
        //                    IsRecurring = false
        //                });
        //            }
        //        }
        //    }

        //    db.SaveChanges();
        //}

        // ── Forgot Password GET ──
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // ── Forgot Password POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = db.Users.FirstOrDefault(u => u.Email == model.Email);

            // Always show success — never reveal if email exists
            if (user != null)
            {
                // Generate secure token
                string token = Guid.NewGuid().ToString("N") +
                                Guid.NewGuid().ToString("N");

                user.ResetToken = token;
                user.ResetTokenExpiry = DateTime.Now.AddHours(1);
                db.SaveChanges();

                // Build reset link
                string resetLink = Url.Action(
                    "ResetPassword", "Account",
                    new { token = token },
                    Request.Url.Scheme
                );

                EmailService.SendPasswordResetEmail(user.Email, user.FirstName, resetLink);
            }

            TempData["ForgotSuccess"] = "true";
            return RedirectToAction("ForgotPassword");
        }

        // ── Reset Password GET ──
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            // Validate token exists and is not expired
            var user = db.Users.FirstOrDefault(u =>
                u.ResetToken == token &&
                u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                TempData["ResetError"] = "This reset link is invalid or has expired.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        // ── Reset Password POST ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = db.Users.FirstOrDefault(u =>
                u.ResetToken == model.Token &&
                u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                TempData["ResetError"] = "This reset link is invalid or has expired.";
                return RedirectToAction("ForgotPassword");
            }

            // Update password and clear token
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            db.SaveChanges();

            TempData["ResetSuccess"] = "Your password has been reset successfully.";
            return RedirectToAction("Login");
        }
    }
}