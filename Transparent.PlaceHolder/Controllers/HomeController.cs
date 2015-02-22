using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data;
using Transparent.Data.Models;

namespace Transparent.PlaceHolder.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Subscribe(Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                var db = new UsersContext();
                if (db.Subscriptions.Any(sub => sub.Email == subscription.Email))
                    return View();
                db.Subscriptions.Add(subscription);
                db.SaveChanges();
                return View();
            }

            // If we got this far the model is invalid.  Redisplay form.
            return View("Index", subscription);
        }
    }
}
