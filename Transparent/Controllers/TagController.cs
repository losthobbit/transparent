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
        private ITags tags;

        public TagController(ITags tags)
        {
            this.tags = tags;
        }

        //
        // GET: /Tag/Details/5

        public ActionResult Details(int id = 0)
        {
            Tag tag = id == 0 ? tags.Root : tags.Find(id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }
    }
}
