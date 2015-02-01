using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Transparent.Data;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Queries;
using Transparent.Data.ViewModels;
using WebMatrix.WebData;

namespace Transparent.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private IUsersContext db;
        private ITickets tickets;
        private readonly IConfiguration configuration;
        private readonly ITags tags;

        public TicketController(IConfiguration configuration, ITags tags, IUsersContext db, ITickets tickets)
        {
            this.configuration = configuration;
            this.tags = tags;
            this.db = db;
            this.tickets = tickets;
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
            var newRank = tickets.SetRank(ticket.Id, ticketRank, WebSecurity.CurrentUserId);
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
            return View(tickets.RaisedBy(ticketsContainer, WebSecurity.CurrentUserId));
        }

        public ActionResult MyQueue(TicketsContainer ticketsContainer)
        {
            return View(tickets.MyQueue(ticketsContainer, WebSecurity.CurrentUserId));
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

        public ActionResult Create(TicketType ticketType = TicketType.Suggestion)
        {
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View("Create", new TicketViewModel(ticketType));
        }

        //
        // POST: /Ticket/Create
        
        private ActionResult Create<TTicket>(TTicket ticket, IDbSet<TTicket> dbSet)
            where TTicket : Ticket, new()
        {
            if (ModelState.IsValid)
            {
                ticket.User = db.UserProfiles.Single(userProfile => userProfile.UserName == User.Identity.Name);
                ticket.CreatedDate = DateTime.UtcNow;
                dbSet.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticket.Id });
            }

            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName", ticket.FkUserId);
            return View(ticket);
        }

        [HttpPost]
        public ActionResult CreateQuestion(TicketViewModel<Question> question)
        {
            return Create(question.Ticket, db.Questions);
        }

        [HttpPost]
        public ActionResult CreateSuggestion(TicketViewModel<Suggestion> suggestion)
        {
            return Create(suggestion.Ticket, db.Suggestions);
        }

        [HttpPost]
        public ActionResult CreateTest(TicketViewModel<Test> test)
        {
            return Create(test.Ticket, db.Tests);
        }

        [HttpPost]
        public ActionResult TakeTest(TestAndAnswerViewModel testAndAnswerViewModel)
        {
            tickets.AnswerTest(testAndAnswerViewModel.Test.Id, testAndAnswerViewModel.Answer, WebSecurity.CurrentUserId);

            return RedirectToAction("Details", "Tag", new { Id = testAndAnswerViewModel.Test.TagId });
        }

        [HttpGet]
        public ActionResult TakeTest(int tagId)
        {
            // find a random test that the user has not yet taken
            var test = tickets.GetRandomUntakenTest(tagId, WebSecurity.CurrentUserId);
            if (test == null)
                throw new NotSupportedException("No more tests are available.");

            // record that the user started the test and deduct points
            tickets.StartTest(test, WebSecurity.CurrentUserId);

            return View(new TestAndAnswerViewModel(test));
        }

        /// <summary>
        /// Get a list of tests that the user can mark.
        /// </summary>
        /// <returns>A list of tests that the user can mark.</returns>
        public ActionResult MarkTests(AnsweredTests answeredTests)
        {
            return View(tickets.GetTestsToBeMarked(answeredTests, WebSecurity.CurrentUserId));
        }

        public ActionResult MarkTest(int userPointId)
        {
            var test = tickets.GetTestToBeMarked(userPointId, WebSecurity.CurrentUserId);

            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkTest(TestAndAnswerViewModel testAndAnswer)
        {
            tickets.MarkTest(testAndAnswer.Id, testAndAnswer.Passed.Value, WebSecurity.CurrentUserId);

            return RedirectToAction("MarkTests");
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

        [HttpGet]
        public ActionResult Volunteer()
        {
            var volunteerViewModel = new VolunteerViewModel
            {
                Volunteer = User.IsInRole(Constants.VolunteerRole),
                Services = db.UserProfiles.Single(user => user.UserName == User.Identity.Name).Services
            };

            return View(volunteerViewModel);
        }

        [HttpPost]
        public ActionResult Volunteer(VolunteerViewModel volunteerViewModel)
        {
            bool wasVolunteer = User.IsInRole(Constants.VolunteerRole);
            if(volunteerViewModel.Volunteer && !wasVolunteer)
            {
                Roles.AddUserToRole(User.Identity.Name, Constants.VolunteerRole);
            }
            else
                if(!volunteerViewModel.Volunteer && wasVolunteer)
                {
                    Roles.RemoveUserFromRole(User.Identity.Name, Constants.VolunteerRole);
                }
            db.UserProfiles.Single(user => user.UserName == User.Identity.Name).Services = volunteerViewModel.Services;
            db.SaveChanges();

            return View(volunteerViewModel);
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