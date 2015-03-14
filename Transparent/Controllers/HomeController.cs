using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.Data.ViewModels;

namespace Transparent.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGeneral general;

        public HomeController(IGeneral general)
        {
            this.general = general;
        }

        /// <summary>
        /// Simple check to see that things are working.  Also useful to warm up the application and trigger
        /// any timed events which need to run.
        /// </summary>
        [HttpGet]
        public string Ping()
        {
            return "hello";
        }

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("HighestRanked", "Ticket");
            return RedirectToAction("Login", "Account");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public PartialViewResult _Stats()
        {
            return PartialView("_StatsPartial", general.GetStats());
        }
    }
}
