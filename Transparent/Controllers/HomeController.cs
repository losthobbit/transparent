using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.Business.ViewModels;
using Transparent.Business.Interfaces;

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
            return View();
        }

        public ActionResult HowItWorks()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult MainGoal()
        {
            return View();
        }

        public ActionResult Subgoal()
        {
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
