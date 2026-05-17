using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSpend.Controllers
{
    public class CurrencyController : Controller
    {
        private bool IsLoggedIn() => Session["UserId"] != null;

        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            ViewBag.UserName = Session["UserName"];
            return View();
        }
    }
}