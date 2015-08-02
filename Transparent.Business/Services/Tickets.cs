using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Common;
using Transparent.Business.ViewModels;
using System.Security;

namespace Transparent.Business.Services
{
    using Data.Extensions;
    using Transparent.Business.Interfaces;
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Contains methods for getting tickets.
    /// </summary>
    /// <remarks>
    /// This started as a way to get tickets, but has now become the way to get pretty much anything...
    /// It may be worth renaming at some point. 
    /// This must be transient, because injecting IUserContext is not safe.
    /// I might also want to consider creating the context at the start of a transaction in case I save an interrupted transaction.
    /// See http://stackoverflow.com/questions/10585478/one-dbcontext-per-web-request-why
    /// </remarks>
    public class Tickets: ITickets
    {
        private readonly IUsersContext db;
        private readonly IDataService dataService;
        private readonly IConfiguration configuration;
        private readonly IUser userService;
        //private readonly ITags tags;

        public Tickets(IUsersContext db, IDataService dataService, IConfiguration configuration, IUser userService/*, ITags tags*/)
        {
            this.db = db;
            this.dataService = dataService;
            this.configuration = configuration;
            this.userService = userService;
            //this.tags = tags;
        }

        private IQueryable<Ticket> TicketsByType(TicketType? ticketType)
        {
            if (ticketType == null)
                return db.Tickets;
            switch (ticketType.Value)
            {
                case TicketType.Question: return db.Questions;
                case TicketType.Suggestion: return db.Suggestions;
                case TicketType.Test: return db.Tests;
            }
            throw new NotSupportedException("Unknown ticket type");
        }

        /// <summary>
        /// Returns a filtered query for tickets based on TicketType and TicketState.
        /// </summary>
        /// <returns>Filtered query for tickets</returns>
        private IQueryable<Ticket> TicketSet(TicketsContainer filter)
        {
            var query = TicketsByType(filter.TicketType);

            if (filter.TicketState.HasValue)
            {
                return query.Where(ticket => ticket.State == filter.TicketState.Value);
            }

            return query;
        }

        public void Create(Ticket ticket, int userId)
        {
            ticket.FkUserId = userId;
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.ModifiedDate = DateTime.UtcNow;
            db.Tickets.Add(ticket);
            CreateFirstBadge(Badge.FirstTicketCreated, userId, ticket.Id);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns list of tickets that have the same tag as the user.
        /// </summary>
        public TicketsContainer MyQueue(TicketsContainer filter, int userId)
        {
            var ticketSet = TicketSet(filter);

            var assignedTickets = from ticket in ticketSet
                                  where new[] { TicketState.Accepted, TicketState.InProgress }.Contains(ticket.State)
                                  && ticket.FkAssignedUserId == userId
                                  select ticket;

            var publicTickets = from ticket in ticketSet.GetPublic()
                                join ticketTag in db.TicketTags on ticket equals ticketTag.Ticket
                                join userTag in db.UserTags on ticketTag.Tag equals userTag.Tag
                                where userTag.FkUserId == userId && userTag.TotalPoints >= userTag.Tag.CompetentPoints
                                select ticket;

            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    assignedTickets.Union(publicTickets)
                ).OrderByDescending(ticket => ticket.Rank)                
            );
        }

        public TicketsContainer RaisedBy(TicketsContainer filter, int userId)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in TicketSet(filter)
                    where ticket.FkUserId == userId
                    select ticket
                ).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        /// <summary>
        /// Returns tickets based on the filter's TicketType and TicketState, sorted by created date in descending order.
        /// </summary>
        /// <returns>Tickets based on the filter's TicketType and TicketState, sorted by created date in descending order</returns>
        public TicketsContainer Newest(TicketsContainer filter, bool publicOnly = false)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    publicOnly
                        ? TicketSet(filter).GetPublic()
                        : TicketSet(filter)
                )
                .OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public TicketsContainer HighestRanked(TicketsContainer filter, bool publicOnly = false)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    publicOnly 
                        ? TicketSet(filter).GetPublic()
                        : TicketSet(filter)
                )
                .OrderByDescending(ticket => ticket.Rank)
                .ThenByDescending(ticket => ticket.State)
                .ThenByDescending(ticket => ticket.ModifiedDate)
            );
        }

        /// <summary>
        /// Returns questions in the completed state.
        /// </summary>
        /// <returns>Questions in the completed state.</returns>
        public TicketsContainer Answered(TicketsContainer filter)
        {
            filter.TicketType = TicketType.Question;
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in TicketSet(filter)
                    where ticket.State == TicketState.Completed
                    select ticket
                )
                .OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public Search Search(Search filter)
        {
            if (String.IsNullOrWhiteSpace(filter.SearchString))
                return null;
            return (Search)filter.Initialize
            (
                filter.ApplyFilter
                (
                    TicketSet(filter)
                    .ExcludeComplete()
                    .ExcludeDraft()
                )
                .OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        /// <summary>
        /// Adjusts the rank of the ticket based on the user's stance.
        /// </summary>
        /// <returns>
        /// The new rank
        /// </returns>
        public int SetRank(int ticketId, Stance ticketRank, int userId)
        {
            var rank = dataService.SetRank(db, ticketId, ticketRank, userId);
            db.SaveChanges();
            return rank;
        }

        /// <summary>
        /// Assigns a ticket to in progress, completed or accepted.
        /// </summary>
        /// <exception cref="NotSupportedException">Ticket is not a suggestion.</exception>
        public AssignViewModel Assign(AssignViewModel assign, int userId)
        {
            var ticket = db.Tickets.Single(t => t.Id == assign.TicketId);

            if (ticket.TicketType != TicketType.Suggestion)
                throw new NotSupportedException("Only suggestions can be assigned.");

            if (ticket.State != assign.TicketState || ticket.FkAssignedUserId != userId)
            {
                ticket.State = assign.TicketState;
                ticket.FkAssignedUserId = userId;
                ticket.History.Add
                (
                    new TicketHistory
                    {
                        Date = DateTime.UtcNow,
                        FkUserId = userId,
                        State = assign.TicketState
                    }
                );
                db.SaveChanges();
            }
            assign.Username = db.UserProfiles.Single(u => u.UserId == userId).UserName;
            return assign;
        }

        /// <summary>
        /// Adjusts the votes of the ticket based on the user's stance.
        /// </summary>
        /// <exception cref="NotSupportedException">Ticket not in the voting state or user is not competent.</exception>
        public VoteViewModel SetVote(int ticketId, Stance vote, int userId)
        {
            var ticket = db.Tickets.Single(t => t.Id == ticketId);

            if (ticket.State != TicketState.Voting)
                throw new NotSupportedException("Only tickets in the Voting state can be voted on.");

            if (!UserHasCompetence(ticket.Id, userId))
                throw new NotSupportedException("User is not competent in any of the ticket tags and cannot vote on the ticket.");

            dataService.SetVote(ticket, vote, userId);
            db.SaveChanges();
            return new VoteViewModel
            {
                 TicketId = ticketId,
                 UserVote = vote,
                 VotesFor = ticket.VotesFor,
                 VotesAgainst = ticket.VotesAgainst,
                 UserMayVote = true
            };
        }

        /// <exception cref="NotSupportedException">User is not competent in parent tag.</exception>
        public IEnumerable<Test> GetUntakenTests(int tagId, int userId)
        {
            if (userService.GetIncompetentParentsTags(userId, tagId).Any())
                throw new NotSupportedException("User is not competent in parent tag.");

            var untakenTests = from test in db.Tests
                               where test.State == TicketState.Completed && test.TicketTags.Any(tag => tag.FkTagId == tagId)
                               from userPoint in db.UserPoints
                                    .Where(point => point.TestTaken == test && point.FkUserId == userId && point.Reason == PointReason.TookTest)
                                    .DefaultIfEmpty()
                               where userPoint == null
                               select test;
            return untakenTests;
        }

        public int CountUntakenTestsRemaining(int tagId, int userId)
        {
            return GetUntakenTests(tagId, userId).Count();
        }

        public Test GetRandomUntakenTest(int tagId, int userId)
        {
            return GetUntakenTests(tagId, userId).Random();
        }

        /// <summary>
        /// Record that the user started the test and deduct points
        /// </summary>
        /// <param name="test">The test to start.</param>
        /// <exception cref="NotSupportedException">Test already completed or user is not competent in parent tag.</exception>
        /// <exception cref="ArgumentNullException">Required argument is null.</exception>
        public void StartTest(Test test, int userId)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            if (userService.GetIncompetentParentsTags(userId, test.TagId).Any())
                throw new NotSupportedException("User is not competent in parent tag.");

            var userPoint = db.UserPoints.SingleOrDefault(point => 
                point.FkTestId == test.Id && 
                point.FkUserId == userId &&
                point.Reason == PointReason.TookTest);
            if (userPoint == null)
            {
                var user = db.UserProfiles.Single(userProfile => userProfile.UserId == userId);
                userPoint = new UserPoint 
                { 
                    FkTestId = test.Id, 
                    FkTagId = test.TagId, 
                    User = user, 
                    Reason = PointReason.TookTest
                };
                var userTag = user.Tags.SingleOrDefault(tag => tag.FkTagId == test.TagId);
                if (userTag == null)
                {
                    userTag = new UserTag { FkTagId = test.TagId, FkUserId = user.UserId };
                    db.UserTags.Add(userTag);
                }
                if(userTag.TotalPoints >= configuration.PointsRequiredBeforeDeductingPoints)
                {
                    dataService.AddPoints(userPoint, userTag, -configuration.PointsToDeductWhenStartingTest);
                }
                db.UserPoints.Add(userPoint);
                db.SaveChanges();
            }
            else
            {
                // Test already started
                if (userPoint.Answer != null)
                {
                    // Test already completed
                    throw new NotSupportedException("The test was already completed.  It cannot be taken again.");
                }
            }
        }

        /// <exception cref="NotSupportedException">Test not started or already completed.</exception>
        public void AnswerTest(int testId, string answer, int userId)
        {
            var userPoint = db.UserPoints.SingleOrDefault(point => 
                point.FkTestId == testId && 
                point.FkUserId == userId &&
                point.Reason == PointReason.TookTest);
            if (userPoint == null)
                throw new NotSupportedException("The test has not started.  It cannot be answered.");
            if (userPoint.Answer != null)
                throw new NotSupportedException("The test has already been answered.  It cannot be answered again.");
            userPoint.Answer = answer;
            CreateFirstBadge(Badge.FirstTestAnswered, userId);
            db.SaveChanges();
        }

        /// <summary>
        /// Check if user is competent in at least one of the tags on the ticket
        /// </summary>
        public bool UserHasCompetence(int ticketId, int userId)
        {
            var ticketTagIds = db.TicketTags.Where(tag => tag.FkTicketId == ticketId).Select(tag => tag.FkTagId);
            return GetCompetentTags(userId).Select(tag => tag.Id).Intersect(ticketTagIds).Any();
        }

        /// <summary>
        /// Check if user is an expert in at least one of the tags on the ticket
        /// </summary>
        public bool UserHasExpertise(int ticketId, int userId)
        {
            var ticketTagIds = db.TicketTags.Where(tag => tag.FkTicketId == ticketId).Select(tag => tag.FkTagId);
            return GetExpertTags(userId).Select(tag => tag.Id).Intersect(ticketTagIds).Any();
        }

        /// <summary>
        /// Gets tags that the user is competent in.
        /// </summary>
        /// <param name="userId">ID of the user.  -1 is not logged in.</param>
        /// <remarks>
        /// For performance purposes, GetExpertTags and GetCompetentTags could be combined into one DB call.
        /// </remarks>
        public IQueryable<Tag> GetCompetentTags(int userId)
        {
            var usersCompetentTags =
                db.UserTags
                .Where(userTag => userTag.FkUserId == userId && userTag.TotalPoints >= userTag.Tag.CompetentPoints)
                .Select(userTag => userTag.Tag);

            if(userId > -1)
                return usersCompetentTags.Union(db.Tags.Where(tag => tag.CompetentPoints == 0));

            return usersCompetentTags;
        }

        /// <summary>
        /// Gets tags that the user is an expert in.
        /// </summary>
        /// <param name="userId">ID of the user.  -1 is not logged in.</param>
        /// <remarks>
        /// For performance purposes, GetExpertTags and GetCompetentTags could be combined into one DB call.
        /// </remarks>
        public IQueryable<Tag> GetExpertTags(int userId)
        {
            var usersExpertTags =
                db.UserTags
                .Where(userTag => userTag.FkUserId == userId && userTag.TotalPoints >= userTag.Tag.ExpertPoints)
                .Select(userTag => userTag.Tag);

            if(userId > -1)
                return usersExpertTags.Union(db.Tags.Where(tag => tag.ExpertPoints == 0));

            return usersExpertTags;
        }

        public IQueryable<TestAndAnswerViewModel> GetTestsToBeMarked(int markersUserId, IQueryable<UserPoint> userPoints)
        {
            var validTags = GetCompetentTags(markersUserId);

            return from userPoint in userPoints
                where userPoint.FkUserId != markersUserId
                && !userPoint.MarkingComplete
                && userPoint.TestMarkings.All(marking => marking.FkUserId != markersUserId)
                && userPoint.Answer != null
                && validTags.Contains(userPoint.Tag)
                select new TestAndAnswerViewModel { Test = userPoint.TestTaken, Answer = userPoint.Answer, Id = userPoint.Id };
        }

        public AnsweredTests GetTestsToBeMarked(AnsweredTests filter, int markersUserId)
        {
            var tests = GetTestsToBeMarked(markersUserId, db.UserPoints);

            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    // TODO: check if this creates an inefficient query
                    tests
                ).OrderByDescending(test => test.Id)
            );
        }

        /// <summary>
        /// Gets the test and answer and validates that the user may mark the test.
        /// </summary>
        /// <param name="userPointId">The ID of the UserPoint record containing the answer to the test.</param>
        /// <exception cref="SecurityException">The user may not mark the test.</exception>
        public TestAndAnswerViewModel GetTestToBeMarked(int userPointId, int markersUserId)
        {
            try
            {
                return GetTestsToBeMarked(markersUserId, db.UserPoints.Where(userPoint => userPoint.Id == userPointId)).Single();
            }
            catch (InvalidOperationException e)
            {
                throw new SecurityException("The user may not mark the test.", e);
            }
        }

        /// <summary>
        /// Marks a test.
        /// </summary>
        /// <param name="userPointId">The ID of the UserPoint record containing the answer to the test.</param>
        /// <param name="passed">Whether the marker passed or failed the test</param>
        /// <exception cref="SecurityException">The user may not mark the test.</exception>
        public void MarkTest(int userPointId, bool passed, int markersUserId)
        {
            // Validate that the user may mark the test
            IQueryable<UserPoint> answeredTestQuery;
            try
            {
                answeredTestQuery = db.UserPoints.Where(point => point.Id == userPointId);
                GetTestsToBeMarked(markersUserId, answeredTestQuery).Single();
            }
            catch (InvalidOperationException e)
            {
                throw new SecurityException("The user may not mark the test.", e);
            }
            var userPoint = answeredTestQuery.Single();
            userPoint.TestMarkings.Add(new TestMarking { FkUserPointId = userPointId, FkUserId = markersUserId, Passed = passed });
            if (userPoint.TestMarkings.Count() >= configuration.MarkersRequiredPerTest)
            {
                TestMarkingCompleted(userPoint);
            }
            CreateFirstBadge(Badge.FirstTestMarked, markersUserId);
            db.SaveChanges();
        }

        public Ticket FindTicket(int id)
        {
            return db.Tickets.Find(id);
        }

        /// <summary>
        /// Creates a first badge and adds points if the user doesn't already have the badge.
        /// </summary>
        /// <remarks>
        /// Does not call SaveChanges
        /// </remarks>
        private void CreateFirstBadge(Badge badge, int userId, int? ticketId = null)
        {
            var user = db.UserProfiles.Single(u => u.UserId == userId);
            if (!user.HasBadge(badge))
            {
                user.SetBadge(badge);
                dataService.AddApplicationPoints(db, userId, configuration.DiPointsForFirstBadge, badge, ticketId);
            }
        }

        /// <summary>
        /// Returns information about the ticket tag.
        /// </summary>
        /// <param name="userId">ID of the user.  -1 means not logged in, therefore no privileges.</param>
        private TicketTagViewModel GetTicketTagInfo(TicketState ticketState,
            TicketTag ticketTag, int ticketUserId, int userId,
            IEnumerable<Tag> competentTags, IEnumerable<Tag> expertTags = null)
        {
            var info = new TicketTagViewModel
            {
                TagId = ticketTag.FkTagId
            };

            info.UserMayDelete = ticketState == TicketState.Verification && !ticketTag.Verified &&
                competentTags.Any(competentTag => competentTag.Id == ticketTag.FkTagId);

            info.UserMayVerify = info.UserMayDelete && ticketTag.FkCreatedById != userId && 
                !(ticketTag.FkCreatedById == null && ticketUserId == userId);

            info.UserIsExpert = expertTags != null && expertTags.Any(expertTag => expertTag.Id == ticketTag.FkTagId);

            return info;
        }

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(int ticketId, int userId)
        {
            return GetTicketTagInfoList(db.Tickets.Single(ticket => ticket.Id == ticketId), userId);
        }

        /// <summary>
        /// Returns information about the ticket tags for the ticket.
        /// </summary>
        /// <param name="userId">ID of the user.  -1 means not logged in, therefore no privileges.</param>
        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(Ticket ticket, int userId)
        {
            var competentTags = GetCompetentTags(userId).ToList();
            var expertTags = GetExpertTags(userId).ToList();
            return ticket.TicketTags.Select(ticketTag => 
                GetTicketTagInfo(ticket.State, ticketTag, ticket.FkUserId, userId, competentTags, expertTags)).ToList();
        }

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        public void DeleteTicketTag(int ticketId, int tagId, int userId)
        {
            db.TicketTags.Remove(GetDeleteableTicketTag(ticketId, tagId, userId));
            SetModifiedDate(ticketId);
            db.SaveChanges();
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        public void VerifyTicketTag(int ticketId, int tagId, int userId)
        {
            GetVerifyableTicketTag(ticketId, tagId, userId).Verified = true;
            SetModifiedDate(ticketId);
            db.SaveChanges();
        }

        /// <exception cref="NotSupportedException">User may not add tag.</exception>
        public void AddTicketTag(int ticketId, int tagId, int userId)
        {
            if (db.Tickets.Single(ticket => ticket.Id == ticketId).State != TicketState.Verification)
                throw new NotSupportedException("Unable to add tag because ticket is not in Verification state");

            if (!GetCompetentTags(userId).Any(tag => tag.Id == tagId))
                throw new NotSupportedException("User may not add tag");

            db.TicketTags.Add(new TicketTag { FkTicketId = ticketId, FkTagId = tagId, FkCreatedById = userId });
            SetModifiedDate(ticketId);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotSupportedException("Only one of a particular tag can be added.", e);
            }
        }

        /// <exception cref="NotSupportedException">User may not answer ticket.</exception>
        public void SetArgument(int ticketId, int userId, string argument)
        {
            if (db.Tickets.Single(ticket => ticket.Id == ticketId).State != TicketState.Discussion)
                throw new NotSupportedException("Argument cannot be set.  Ticket is not in the Discussion state.");

            if (!UserHasExpertise(ticketId, userId))
                throw new NotSupportedException("User is not an expert in any of the ticket tags and cannot answer the ticket.");

            var argumentRow = db.Arguments.SingleOrDefault(arg => arg.FkTicketId == ticketId && arg.FkUserId == userId);
            if (argumentRow == null)
            {
                db.Arguments.Add(new Argument { FkTicketId = ticketId, FkUserId = userId, Body = argument });
                CreateFirstBadge(Badge.FirstArgument, userId, ticketId);
            }
            else
            {
                argumentRow.Body = argument;
            }
            SetModifiedDate(ticketId);

            db.SaveChanges();
        }

        private void SetModifiedDate(int ticketId)
        {
            db.Tickets.Single(ticket => ticket.Id == ticketId).ModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// A test has been marked by the required number of markers.  Set the points.
        /// </summary>
        /// <param name="testAnswer">The answered test.</param>
        private void TestMarkingCompleted(UserPoint testAnswer)
        {
            testAnswer.MarkingComplete = true;

            var numberOfPasses = testAnswer.TestMarkings.Count(marking => marking.Passed);
            bool? testPassed = numberOfPasses * 2 == configuration.MarkersRequiredPerTest
                ? (bool?)null
                : numberOfPasses * 2 > configuration.MarkersRequiredPerTest;

            AdjustTestTakersPoints(testAnswer, testPassed);

            AdjustTestMarkersPoints(testAnswer, testPassed);
        }

        /// <summary>
        /// Adjust the points of a user who took a test, based on whether or not they passed.
        /// </summary>
        private void AdjustTestTakersPoints(UserPoint testAnswer, bool? testPassed)
        {
            int pointsToAdd;
            if (testPassed.HasValue)
            {
                if (testPassed.Value)
                {
                    pointsToAdd = configuration.PointsForPassingATest - testAnswer.Quantity;
                }
                else
                {
                    pointsToAdd = 0;
                }
            }
            else
            {
                // 50 / 50 split - reset points
                pointsToAdd = -testAnswer.Quantity;
            }
            if (pointsToAdd != 0)
                dataService.AddPoints
                (
                    testAnswer,
                    testAnswer.User.Tags.Single(userTag => userTag.FkTagId == testAnswer.FkTagId),
                    pointsToAdd
                );
        }

        /// <summary>
        /// Adjust the points of a test marker, based on whether they agreed with the minority or majority.
        /// </summary>
        /// <param name="testAnswer">The answered test.</param>
        private void AdjustTestMarkersPoints(UserPoint testAnswer, bool? testPassed)
        {
            // Deduct markers' points in the case of a tie, or minority
            dataService.AddPoints(db, testAnswer.TestMarkings
                .Where(testMarking => testPassed == null || testMarking.Passed != testPassed.Value)
                .Select(marking => marking.FkUserId), testAnswer.FkTagId, 
                -configuration.PointsMarkersLoseForDisagreeingATestResult, PointReason.MarkedTest,
                testAnswer.FkTestId);

            // Increase markers' points in the case of a majority
            dataService.AddPoints(db, testAnswer.TestMarkings
                .Where(testMarking => testPassed.HasValue && testMarking.Passed == testPassed.Value)
                .Select(marking => marking.FkUserId), testAnswer.FkTagId,  
                configuration.PointsMarkersGainForAgreeingATestResult, PointReason.MarkedTest,
                testAnswer.FkTestId);
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        private TicketTag GetTicketTag(int ticketId, int tagId, int userId, bool userDeletable, bool userVerifyable)
        {
            var ticket = FindTicket(ticketId);
            var ticketTag = ticket.TicketTags.First(tag => tag.FkTagId == tagId);
            var ticketTagInfo = GetTicketTagInfo(ticket.State, ticketTag, ticket.FkUserId, userId, GetCompetentTags(userId));
            if (userDeletable && !ticketTagInfo.UserMayDelete)
            {
                throw new NotSupportedException("User may not delete tag.");
            }
            if (userVerifyable && !ticketTagInfo.UserMayVerify)
            {
                throw new NotSupportedException("User may not verify tag.");
            }
            return ticketTag;
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        private TicketTag GetVerifyableTicketTag(int ticketId, int tagId, int userId)
        {
            return GetTicketTag(ticketId, tagId, userId, false, true);
        }

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        private TicketTag GetDeleteableTicketTag(int ticketId, int tagId, int userId)
        {
            return GetTicketTag(ticketId, tagId, userId, true, false);
        }
    }
}
