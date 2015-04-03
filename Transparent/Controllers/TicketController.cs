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
using Transparent.Business.ViewModels;
using Transparent.Business.Interfaces;
using Transparent.Business.Maps;
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
        [ActionName("_Discuss")]
        public PartialViewResult Post_Discuss(ArgumentViewModel discuss)
        {
            if (ModelState.IsValid)
            {
                tickets.SetArgument(discuss.FkTicketId, WebSecurity.CurrentUserId, discuss.Body);
            }

            return PartialView("_DiscussEditPartial", discuss);
        }

        [HttpGet]
        [ActionName("_Discuss")]
        public PartialViewResult Get_Discuss(ArgumentViewModel discuss)
        {
            return PartialView("_DiscussEditPartial", discuss);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _SetRank(TicketDetailsViewModel ticket)
        {
            var newRank = tickets.SetRank(ticket.Id, ticket.NewRank, WebSecurity.CurrentUserId);
            ticket.Rank = newRank;
            ticket.UserRank = ticket.NewRank;
            return PartialView("_RankPartial", ticket);
        }

        public PartialViewResult _Rank(TicketDetailsViewModel ticket)
        {
            return PartialView("_RankPartial", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _SetVote(VoteViewModel vote)
        {
            var newVote = tickets.SetVote(vote.TicketId, vote.NewVote, WebSecurity.CurrentUserId);
            return PartialView("_VotePartial", newVote);
        }

        public PartialViewResult _Vote(VoteViewModel vote)
        {
            return PartialView("_VotePartial", vote);
        }

        public PartialViewResult _TicketTags(TicketTagsViewModel ticket)
        {
            return PartialView("_TicketTagsPartial", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _VerifyTag(TicketTagsViewModel ticket)
        {
            var userId = WebSecurity.CurrentUserId;
            if (ticket.DeleteTagId != null)
                tickets.DeleteTicketTag(ticket.TicketId, ticket.DeleteTagId.Value, userId);
            else
                if (ticket.VerifyTagId != null)
                    tickets.VerifyTicketTag(ticket.TicketId, ticket.VerifyTagId.Value, userId);

            // Probably the worst way to get this list... guess that happens when one codes past midnight.
            ticket.TagInfo = TicketTagViewModel.CreateList(tickets.FindTicket(ticket.TicketId), 
                tickets.GetTicketTagInfoList(ticket.TicketId, userId));

            return _TicketTags(ticket);
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
            Ticket ticket = tickets.FindTicket(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var ticketDetailsViewModel = ticket.Map(WebSecurity.CurrentUserId).Map(ticket.GetUserRank(WebSecurity.CurrentUserId));
            ticketDetailsViewModel.TagInfo = tickets.GetTicketTagInfoList(ticket, WebSecurity.CurrentUserId);
            return View(ticketDetailsViewModel);
        }

        //
        // GET: /Ticket/Create

        public ActionResult Create(TicketType ticketType = TicketType.Suggestion)
        {
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View("Create", new CreateTicketViewModel(ticketType));
        }

        //
        // POST: /Ticket/Create
        
        private ActionResult Create<TTicket>(TTicket ticket)
            where TTicket : Ticket, new()
        {
            if (ModelState.IsValid)
            {
                tickets.Create(ticket, WebSecurity.CurrentUserId);
                return RedirectToAction("Details", new { id = ticket.Id });
            }

            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName", ticket.FkUserId);
            return View(ticket);
        }

        [HttpPost]
        public ActionResult CreateQuestion(CreateTicketViewModel<Question> question)
        {
            return Create(question.Ticket);
        }

        [HttpPost]
        public ActionResult CreateSuggestion(CreateTicketViewModel<Suggestion> suggestion)
        {
            return Create(suggestion.Ticket);
        }

        [HttpPost]
        public ActionResult CreateTest(CreateTicketViewModel<Test> test)
        {
            return Create(test.Ticket);
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