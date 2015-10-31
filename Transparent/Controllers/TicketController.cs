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
        [Authorize]
        public PartialViewResult Post_Discuss(ArgumentViewModel discuss)
        {
            if (ModelState.IsValid)
            {
                tickets.SetArgument(discuss.FkTicketId, WebSecurity.CurrentUserId, discuss.Body);
                discuss.Message = "Saved";
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
        public ActionResult _SetRank(TicketDetailsViewModel ticket)
        {
            if(!WebSecurity.IsAuthenticated)
                return Json(new { unauthorized = true });

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
        [Authorize]
        public PartialViewResult _SetVote(VoteViewModel vote)
        {
            var newVote = tickets.SetVote(vote.TicketId, vote.NewVote, WebSecurity.CurrentUserId);
            return PartialView("_VotePartial", newVote);
        }

        public PartialViewResult _Vote(VoteViewModel vote)
        {
            return PartialView("_VotePartial", vote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public PartialViewResult _SetAssign(AssignViewModel assign)
        {
            var newVote = tickets.Assign(assign, WebSecurity.CurrentUserId);
            return PartialView("_AssignPartial", assign);
        }

        [HttpGet]
        public PartialViewResult _Assign(AssignViewModel assign)
        {
            return PartialView("_AssignPartial", assign);
        }

        public PartialViewResult _TicketTags(TicketTagsViewModel ticket)
        {
            return PartialView("_TicketTagsPartial", ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public PartialViewResult _VerifyTag(TicketTagsViewModel ticket)
        {
            var userId = WebSecurity.CurrentUserId;

            var stanceDetail = new StanceDetail(ticket.ForId, ticket.AgainstId, ticket.NeutralId);

            tickets.VoteForTicketTag(stanceDetail.Stance, ticket.TicketId, stanceDetail.Id, userId);

            // Probably the worst way to get this list... guess that happens when one codes past midnight.
            ticket.TagInfo = TicketTagViewModel.CreateList(tickets.FindTicket(ticket.TicketId), 
                tickets.GetTicketTagInfoList(ticket.TicketId, userId));

            return _TicketTags(ticket);
        }

        public ActionResult Newest(TicketsContainer ticketsContainer)
        {
            return View(tickets.Newest(ticketsContainer, true));
        }

        [Authorize]
        public ActionResult RaisedByMe(TicketsContainer ticketsContainer)
        {
            return View(tickets.RaisedBy(ticketsContainer, WebSecurity.CurrentUserId));
        }

        [Authorize]
        public ActionResult MyQueue(TicketsContainer ticketsContainer)
        {
            return View(tickets.MyQueue(ticketsContainer, WebSecurity.CurrentUserId));
        }

        public ActionResult HighestRanked(TicketsContainer ticketsContainer)
        {
            return View(tickets.HighestRanked(ticketsContainer, true));
        }

        public ActionResult Search(Search search)
        {
            return View(tickets.Search(search));
        }

        public ActionResult Answered(TicketsContainer ticketsContainer)
        {
            return View(tickets.Answered(ticketsContainer));
        }

        /// <summary>
        /// Returns suggestions which are accepted or in progress.
        /// </summary>
        /// <returns>Suggestions which are accepted or in progress</returns>
        public ActionResult Accepted(TicketsContainer ticketsContainer)
        {
            ticketsContainer.TicketType = TicketType.Suggestion;
            if (ticketsContainer.TicketState == null)
                ticketsContainer.TicketState = TicketState.Accepted;
            return View(tickets.HighestRanked(ticketsContainer));
        }

        /// <summary>
        /// Returns suggestions which are completed or in rejected.
        /// </summary>
        /// <returns>Suggestions which are completed or in rejected</returns>
        public ActionResult Archive(TicketsContainer ticketsContainer)
        {
            ticketsContainer.TicketType = TicketType.Suggestion;
            if (ticketsContainer.TicketState == null)
                ticketsContainer.TicketState = TicketState.Completed;
            return View(tickets.Newest(ticketsContainer));
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

        [Authorize]
        public ActionResult Create(TicketType ticketType = TicketType.Suggestion)
        {
            ViewBag.FkUserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View("Create", new CreateTicketViewModel(ticketType));
        }

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
        [Authorize]
        public ActionResult CreateQuestion(CreateTicketViewModel<Question> question)
        {
            return Create(question.Ticket);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateSuggestion(CreateTicketViewModel<Suggestion> suggestion)
        {
            return Create(suggestion.Ticket);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateTest(CreateTicketViewModel<Test> test)
        {
            return Create(test.Ticket);
        }

        [HttpPost]
        [Authorize]
        public ActionResult TakeTest(TestAndAnswerViewModel testAndAnswerViewModel)
        {
            tickets.AnswerTest(testAndAnswerViewModel.Test.Id, testAndAnswerViewModel.Answer, WebSecurity.CurrentUserId);

            return RedirectToAction("TestTaken", "Tag", new { Id = testAndAnswerViewModel.Test.TagId });
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public ActionResult MarkTests(AnsweredTests answeredTests)
        {
            return View(tickets.GetTestsToBeMarked(answeredTests, WebSecurity.CurrentUserId));
        }

        [Authorize]
        public ActionResult MarkTest(int userPointId)
        {
            var test = tickets.GetTestToBeMarked(userPointId, WebSecurity.CurrentUserId);

            return View(test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult MarkTest(TestAndAnswerViewModel testAndAnswer)
        {
            tickets.MarkTest(testAndAnswer.Id, testAndAnswer.Passed.Value, WebSecurity.CurrentUserId);

            return RedirectToAction("MarkTests");
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