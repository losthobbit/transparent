using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Controllers
{
    public class TagController : Controller
    {
        private IUsersContext db = new UsersContext();

        public TagController()
        {
        }

        //
        // GET: /Tag/Details/5

        public ActionResult Details(int id = 0)
        {
            Tag tag = db.Tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }
    }
}
