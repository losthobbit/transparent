using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Queries;

namespace Transparent.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private IUsersContext db = new UsersContext();
        private Tickets tickets;

        public TicketController()
        {
            tickets = new Tickets(db);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _IncreaseRank(TicketAndUserRank ticket)
        {
            return SetRank(ticket, TicketRank.Up);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _DecreaseRank(TicketAndUserRank ticket)
        {
            return SetRank(ticket, TicketRank.Down);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _CancelRank(TicketAndUserRank ticket)
        {
            return SetRank(ticket, TicketRank.NotRanked);
        }

        private PartialViewResult SetRank(TicketAndUserRank ticket, TicketRank ticketRank)
        {
            // TODO: Ensure user has permission to change rank
            var newRank = tickets.SetRank(ticket.Id, ticketRank, User.Identity.Name);
            db.SaveChanges();
            ticket.Rank = newRank.Item1;
            ticket.UserRank = newRank.Item2;
            return PartialView("_RankPartial", ticket);
        }

        public PartialViewResult _Rank(TicketAndUserRank ticket)
        {
            return PartialView("_RankPartial", ticket);
        }

        public ActionResult Newest(TicketsContainer ticketsContainer)
        {
            return View(tickets.Newest(ticketsContainer));
        }

        public ActionResult RaisedByMe(TicketsContainer ticketsContainer)
        {
            return View(tickets.RaisedBy(ticketsContainer, User.Identity.Name));
        }

        public ActionResult MyQueue(TicketsContainer ticketsContainer)
        {
            return View(tickets.MyQueue(ticketsContainer, User.Identity.Name));
        }

        public ActionResult HighestRanked(TicketsContainer ticketsContainer)
        {
            return View(tickets.HighestRanked(ticketsContainer));
        }

        public ActionResult Search(Search search)
        {
            return View(tickets.Search(search));
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
            return View(new TicketAndUserRank(ticket, ticket.GetTicketRank(User.Identity.Name)));
        }

        //
        // GET: /Ticket/Create

        public ActionResult Create()
        {
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View(new Ticket());
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
            if (db is IDisposable)
            {
                ((IDisposable)db).Dispose();
            }
            base.Dispose(disposing);
        }
    }
}