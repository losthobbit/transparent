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
using Transparent.Data.ViewModels;
using System.Security;

namespace Transparent.Data.Queries
{
    /// <summary>
    /// Contains methods for getting ticket queries.
    /// </summary>
    /// <remarks>
    /// Can be a singleton.
    /// </remarks>
    public class Tickets: ITickets
    {
        /// <summary>
        /// One requires this number of total points on a tag in order to view it in My Queue.
        /// </summary>
        /// <remarks>
        /// Can be a configurable setting.
        /// </remarks>
        public const int MinimumUserTagPointsToWorkOnTicketWithSameTag = 1;

        private IUsersContext usersContext;
        private IDbSet<Ticket> tickets;
        private IDbSet<UserProfile> userProfiles;
        private IDbSet<TicketTag> ticketTags;
        private IDbSet<UserTag> userTags;
        private IDbSet<Test> tests;
        private IDbSet<UserPoint> userPoints;
        private IDbSet<TestMarking> testMarkings;

        private readonly IConfiguration configuration;

        public Tickets(IUsersContext usersContext, IConfiguration configuration)
        {
            this.usersContext = usersContext;
            this.tickets = usersContext.Tickets;
            this.userProfiles = usersContext.UserProfiles;
            this.ticketTags = usersContext.TicketTags;
            this.userTags = usersContext.UserTags;
            this.tests = usersContext.Tests;
            this.userPoints = usersContext.UserPoints;
            this.testMarkings = usersContext.TestMarkings;

            this.configuration = configuration;
        }

        private IQueryable<Ticket> TicketSet(TicketsContainer filter)
        {
            if (filter.TicketType == null)
                return tickets;
            switch (filter.TicketType.Value)
            {
                case TicketType.Question: return usersContext.Questions;
                case TicketType.Suggestion: return usersContext.Suggestions;
                case TicketType.Test: return usersContext.Tests;
            }
            throw new NotSupportedException("Unknown ticket type");
        }

        /// <summary>
        /// Returns list of tickets that have the same tag as the user.
        /// </summary>
        public TicketsContainer MyQueue(TicketsContainer filter, int userId)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in TicketSet(filter)
                    join ticketTag in ticketTags on ticket equals ticketTag.Ticket
                    join userTag in userTags on ticketTag.Tag equals userTag.Tag
                    where userTag.FkUserId == userId && userTag.TotalPoints >= MinimumUserTagPointsToWorkOnTicketWithSameTag
                    select ticket
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

        public TicketsContainer Newest(TicketsContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter(TicketSet(filter)).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public TicketsContainer HighestRanked(TicketsContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter(TicketSet(filter)).OrderByDescending(ticket => ticket.Rank)
            );
        }

        public Search Search(Search filter)
        {
            if (String.IsNullOrWhiteSpace(filter.SearchString))
                return null;
            return (Search)filter.Initialize
            (
                filter.ApplyFilter(TicketSet(filter)).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, int userId)
        {
            var ticket = tickets.Single(t => t.Id == ticketId);
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.FkUserId == userId);
            if(rankRecord == null)
            {
                if(ticketRank != TicketRank.NotRanked)
                {
                    ticket.UserRanks.Add
                    (
                        new TicketUserRank
                        { 
                            Up = ticketRank == TicketRank.Up,
                            FkUserId = userId
                        }
                    );
                    ticket.Rank += (int)ticketRank;
                }
            }
            else
            {
                if(ticketRank == TicketRank.NotRanked)
                {
                    ticket.UserRanks.Remove(rankRecord);
                    ticket.Rank += rankRecord.Up ? -1 : 1;
                }
                else
                {
                    if(rankRecord.Up && ticketRank == TicketRank.Down || !rankRecord.Up && ticketRank == TicketRank.Up)
                    {
                        rankRecord.Up = !rankRecord.Up;
                        ticket.Rank += (int)ticketRank * 2;
                    }
                }
            }
            usersContext.SaveChanges();
            return new Tuple<int, TicketRank>(ticket.Rank, ticketRank);
        }

        public IEnumerable<Test> GetUntakenTests(int tagId, int userId)
        {    
            var untakenTests = from test in tests
                               where test.TicketTags.Any(tag => tag.FkTagId == tagId)
                               from userPoint in userPoints
                                    .Where(point => point.TestTaken == test && point.FkUserId == userId)
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
        /// <exception cref="NotSupportedException">Test already completed.</exception>
        /// <exception cref="ArgumentNullException">Required argument is null.</exception>
        public void StartTest(Test test, int userId)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            var userPoint = userPoints.SingleOrDefault(point => point.FkTestId == test.Id && point.FkUserId == userId);
            if (userPoint == null)
            {
                var user = userProfiles.Single(userProfile => userProfile.UserId == userId);
                userPoint = new UserPoint { FkTestId = test.Id, FkTagId = test.TagId, User = user, Quantity = -configuration.PointsToDeductWhenStartingTest };
                userPoints.Add(userPoint);
                usersContext.SaveChanges();
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
            var userPoint = userPoints.SingleOrDefault(point => point.FkTestId == testId && point.FkUserId == userId);
            if (userPoint == null)
                throw new NotSupportedException("The test has not started.  It cannot be answered.");
            if (userPoint.Answer != null)
                throw new NotSupportedException("The test has already been answered.  It cannot be answered again.");
            userPoint.Answer = answer;
            usersContext.SaveChanges();
        }

        private IQueryable<Tag> GetCompetentTags(int userId)
        {
            return userTags
            .Where(userTag => userTag.FkUserId == userId && userTag.TotalPoints >= configuration.PointsRequiredToBeCompetent)
            .Select(userTag => userTag.Tag);
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
            var tests = GetTestsToBeMarked(markersUserId, userPoints);

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
                return GetTestsToBeMarked(markersUserId, userPoints.Where(userPoint => userPoint.Id == userPointId)).Single();
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
            try
            {
                GetTestsToBeMarked(markersUserId, userPoints.Where(point => point.Id == userPointId)).Single();
            }
            catch (InvalidOperationException e)
            {
                throw new SecurityException("The user may not mark the test.", e);
            }
            testMarkings.Add(new TestMarking { FkUserPointId = userPointId, FkUserId = markersUserId, Passed = passed });
            usersContext.SaveChanges();
        }

        public Ticket FindTicket(int id)
        {
            return usersContext.Tickets.Find(id);
        }

        private TicketTagViewModel GetTicketTagInfo(TicketTag ticketTag, int ticketUserId, int userId, IEnumerable<Tag> competentTags)
        {
            return new TicketTagViewModel
            {
                TagId = ticketTag.FkTagId,
                UserMayVerify = !ticketTag.Verified && ticketTag.FkCreatedById != userId &&
                    !(ticketTag.FkCreatedById == null && ticketUserId == userId) &&
                    competentTags.Any(competentTag => competentTag.Id == ticketTag.FkTagId)
            };
        }

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(int ticketId, int userId)
        {
            return GetTicketTagInfoList(tickets.Single(ticket => ticket.Id == ticketId), userId);
        }

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(Ticket ticket, int userId)
        {
            var competentTags = GetCompetentTags(userId).ToList();
            return ticket.TicketTags.Select(ticketTag => GetTicketTagInfo(ticketTag, ticket.FkUserId, userId, competentTags)).ToList();
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        private TicketTag GetVerifyableTicketTag(int ticketId, int tagId, int userId)
        {
            var ticket = FindTicket(ticketId);
            var ticketTag = ticket.TicketTags.Single(tag => tag.FkTagId == tagId);
            var ticketTagInfo = GetTicketTagInfo(ticketTag, ticket.FkUserId, userId, GetCompetentTags(userId));
            if (!ticketTagInfo.UserMayVerify)
            {
                throw new NotSupportedException("User may not verify tag.");
            }
            return ticketTag;
        }

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        public void DeleteTicketTag(int ticketId, int tagId, int userId)
        {
            ticketTags.Remove(GetVerifyableTicketTag(ticketId, tagId, userId));
            usersContext.SaveChanges();
            // TODO: Some kind of event so that the ticket moves to the next stage when ready.
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        public void VerifyTicketTag(int ticketId, int tagId, int userId)
        {
            GetVerifyableTicketTag(ticketId, tagId, userId).Verified = true;
            usersContext.SaveChanges();
            // TODO: Some kind of event so that the ticket moves to the next stage when ready.
        }
    }
}
