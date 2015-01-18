using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Common;
using Transparent.Data.ViewModels;

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
        public TicketsContainer MyQueue(TicketsContainer filter, string userName)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in TicketSet(filter)
                    join ticketTag in ticketTags on ticket equals ticketTag.Ticket
                    join userTag in userTags on ticketTag.Tag equals userTag.Tag
                    where userTag.User.UserName == userName && userTag.TotalPoints >= MinimumUserTagPointsToWorkOnTicketWithSameTag
                    select ticket
                ).OrderByDescending(ticket => ticket.Rank)                
            );
        }

        public TicketsContainer RaisedBy(TicketsContainer filter, string userName)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in TicketSet(filter)
                    where ticket.User.UserName == userName
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

        public Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, string userName)
        {
            var ticket = tickets.Single(t => t.Id == ticketId);
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.User.UserName == userName);
            if(rankRecord == null)
            {
                if(ticketRank != TicketRank.NotRanked)
                {
                    ticket.UserRanks.Add
                    (
                        new TicketUserRank
                        { 
                            Up = ticketRank == TicketRank.Up,
                            User = userProfiles.Single(user => user.UserName == userName)
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
            return new Tuple<int, TicketRank>(ticket.Rank, ticketRank);
        }

        public IEnumerable<Test> GetUntakenTests(int tagId, string userName)
        {    
            var untakenTests = from test in tests
                               where test.TicketTags.Any(tag => tag.FkTagId == tagId)
                               from userPoint in userPoints
                                    .Where(point => point.TestTaken == test && point.User.UserName == userName)
                                    .DefaultIfEmpty()
                               where userPoint == null
                               select test;
            return untakenTests;
        }

        public int CountUntakenTestsRemaining(int tagId, string userName)
        {
            return GetUntakenTests(tagId, userName).Count();
        }

        public Test GetRandomUntakenTest(int tagId, string userName)
        {
            return GetUntakenTests(tagId, userName).Random();
        }

        /// <summary>
        /// Record that the user started the test and deduct points
        /// </summary>
        /// <param name="test">The test to start.</param>
        /// <exception cref="NotSupportedException">Test already completed.</exception>
        /// <exception cref="ArgumentNullException">Required argument is null.</exception>
        public void StartTest(Test test, string userName)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            var user = userProfiles.Single(userProfile => userProfile.UserName == userName);
            var userPoint = userPoints.SingleOrDefault(point => point.FkTestId == test.Id && point.FkUserId == user.UserId);
            if (userPoint == null)
            {
                userPoint = new UserPoint { FkTestId = test.Id, FkTagId = test.TagId, User = user, Quantity = - configuration.PointsToDeductWhenStartingTest};
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
        public void AnswerTest(int testId, string answer, string userName)
        {
            var userPoint = userPoints.SingleOrDefault(point => point.FkTestId == testId && point.User.UserName == userName);
            if (userPoint == null)
                throw new NotSupportedException("The test has not started.  It cannot be answered.");
            if (userPoint.Answer != null)
                throw new NotSupportedException("The test has already been answered.  It cannot be answered again.");
            userPoint.Answer = answer;
            usersContext.SaveChanges();
        }

        public AnsweredTests TestsToBeMarked(AnsweredTests filter, string userName)
        {
            // TODO: check if this creates an inefficient query
            var tests = from userPoint in userPoints
                        where userPoint.User.UserName != userName
                        select new TestAndAnswerViewModel { Test = userPoint.TestTaken, Answer = userPoint.Answer };

            return new AnsweredTests(tests);
        }
    }
}
