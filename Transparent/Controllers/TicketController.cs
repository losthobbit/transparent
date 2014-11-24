using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Models;
using Transparent.Data.Queries;

namespace Transparent.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private UsersContext db = new UsersContext();
        private Tickets tickets;

        public TicketController()
        {
            tickets = new Tickets(db.Tickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _IncreaseRank(Ticket ticket)
        {
            // TODO: Ensure user has permission to increase rank
            var newRank = tickets.IncreaseRank(ticket.Id);
            db.SaveChanges();
            ticket.Rank = newRank;
            return PartialView("_RankPartial", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _DecreaseRank(Ticket ticket)
        {
            // TODO: Ensure user has permission to increase rank
            var newRank = tickets.DecreaseRank(ticket.Id);
            db.SaveChanges();
            ticket.Rank = newRank;
            return PartialView("_RankPartial", ticket);
        }

        public PartialViewResult _Rank(Ticket ticket)
        {
            return PartialView("_RankPartial", ticket);
        }

        public ActionResult Newest()
        {
            return View(tickets.Newest());
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(Search search)
        {
            return View(tickets.Search(search.SearchString));
        }

        //
        // GET: /Ticket/Details/5

        public ActionResult Details(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // GET: /Ticket/Create

        public ActionResult Create()
        {
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /Ticket/Create

        [HttpPost]
        public ActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.User = db.UserProfiles.Single(userProfile => userProfile.UserName == User.Identity.Name);
                ticket.CreatedDate = DateTime.UtcNow;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticket.Id });
            }

            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName", ticket.FkUserId);
            return View(ticket);
        }

        //
        // GET: /Ticket/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName", ticket.FkUserId);
            return View(ticket);
        }

        //
        // POST: /Ticket/Edit/5

        [HttpPost]
        public ActionResult Edit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName", ticket.FkUserId);
            return View(ticket);
        }

        //
        // GET: /Ticket/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //
        // POST: /Ticket/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Newest");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}