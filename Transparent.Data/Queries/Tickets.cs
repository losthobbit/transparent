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
    using Extensions;

    /// <summary>
    /// Contains methods for getting tickets.
    /// </summary>
    /// <remarks>
    /// This started as a way to get tickets, but has now become the way to get pretty much anything... basically a combination business and data service.
    /// It may be worth renaming at some point.  There's no need to split into a business and data service, unless a different data source is required.
    /// Probably should not be a singleton.  I'm not sure that injecting IUserContext is safe... I don't know if that guarantees a unique context per thread,
    /// nor whether that matters or not.  I might also want to consider creating the context at the start of a transaction in case I save an interrupted
    /// transaction.
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

        private IUsersContext db;

        private readonly IConfiguration configuration;

        public Tickets(IUsersContext db, IConfiguration configuration)
        {
            this.db = db;

            this.configuration = configuration;
        }

        private IQueryable<Ticket> TicketSet(TicketsContainer filter)
        {
            if (filter.TicketType == null)
                return db.Tickets;
            switch (filter.TicketType.Value)
            {
                case TicketType.Question: return db.Questions;
                case TicketType.Suggestion: return db.Suggestions;
                case TicketType.Test: return db.Tests;
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
                    from ticket in TicketSet(filter).GetPublic()
                    join ticketTag in db.TicketTags on ticket equals ticketTag.Ticket
                    join userTag in db.UserTags on ticketTag.Tag equals userTag.Tag
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
                filter.ApplyFilter
                (
                    TicketSet(filter)
                    .GetPublic()
                )
                .OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public TicketsContainer HighestRanked(TicketsContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    TicketSet(filter)
                    .GetPublic()
                )
                .OrderByDescending(ticket => ticket.Rank)
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

        public Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, int userId)
        {
            var ticket = db.Tickets.Single(t => t.Id == ticketId);
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
            db.SaveChanges();
            return new Tuple<int, TicketRank>(ticket.Rank, ticketRank);
        }

        public IEnumerable<Test> GetUntakenTests(int tagId, int userId)
        {
            var untakenTests = from test in db.Tests
                               where test.TicketTags.Any(tag => tag.FkTagId == tagId)
                               from userPoint in db.UserPoints
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
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkTestId == test.Id && point.FkUserId == userId);
            if (userPoint == null)
            {
                var user = db.UserProfiles.Single(userProfile => userProfile.UserId == userId);
                userPoint = new UserPoint { FkTestId = test.Id, FkTagId = test.TagId, User = user, Quantity = -configuration.PointsToDeductWhenStartingTest };
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
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkTestId == testId && point.FkUserId == userId);
            if (userPoint == null)
                throw new NotSupportedException("The test has not started.  It cannot be answered.");
            if (userPoint.Answer != null)
                throw new NotSupportedException("The test has already been answered.  It cannot be answered again.");
            userPoint.Answer = answer;
            db.SaveChanges();
        }

        public IQueryable<Tag> GetCompetentTags(int userId)
        {
            return db.UserTags
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
            try
            {
                GetTestsToBeMarked(markersUserId, db.UserPoints.Where(point => point.Id == userPointId)).Single();
            }
            catch (InvalidOperationException e)
            {
                throw new SecurityException("The user may not mark the test.", e);
            }
            db.TestMarkings.Add(new TestMarking { FkUserPointId = userPointId, FkUserId = markersUserId, Passed = passed });
            db.SaveChanges();
        }

        public Ticket FindTicket(int id)
        {
            return db.Tickets.Find(id);
        }

        private TicketTagViewModel GetTicketTagInfo(TicketTag ticketTag, int ticketUserId, int userId, IEnumerable<Tag> competentTags)
        {
            var info = new TicketTagViewModel
            {
                TagId = ticketTag.FkTagId
            };

            info.UserMayDelete = !ticketTag.Verified &&
                competentTags.Any(competentTag => competentTag.Id == ticketTag.FkTagId);

            info.UserMayVerify = info.UserMayDelete && ticketTag.FkCreatedById != userId && 
                !(ticketTag.FkCreatedById == null && ticketUserId == userId);

            return info;
        }

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(int ticketId, int userId)
        {
            return GetTicketTagInfoList(db.Tickets.Single(ticket => ticket.Id == ticketId), userId);
        }

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(Ticket ticket, int userId)
        {
            var competentTags = GetCompetentTags(userId).ToList();
            return ticket.TicketTags.Select(ticketTag => GetTicketTagInfo(ticketTag, ticket.FkUserId, userId, competentTags)).ToList();
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        private TicketTag GetTicketTag(int ticketId, int tagId, int userId, bool userDeletable, bool userVerifyable)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            var ticket = FindTicket(ticketId);
            var ticketTag = ticket.TicketTags.First(tag => tag.FkTagId == tagId);
            var ticketTagInfo = GetTicketTagInfo(ticketTag, ticket.FkUserId, userId, GetCompetentTags(userId));
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

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        public void DeleteTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            db.TicketTags.Remove(GetDeleteableTicketTag(ticketId, tagId, userId));
            db.SaveChanges();
            // TODO: Some kind of event so that the ticket moves to the next stage when ready.
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        public void VerifyTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            GetVerifyableTicketTag(ticketId, tagId, userId).Verified = true;
            db.SaveChanges();
            // TODO: Some kind of event so that the ticket moves to the next stage when ready.
        }

        /// <exception cref="NotSupportedException">User may not add tag.</exception>
        public void AddTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            if (!GetCompetentTags(userId).Any(tag => tag.Id == tagId))
                throw new NotSupportedException("User may not add tag");

            db.TicketTags.Add(new TicketTag { FkTicketId = ticketId, FkTagId = tagId, FkCreatedById = userId });

            db.SaveChanges();
            // TODO: Some kind of event so that the ticket moves to the next stage when ready.
        }
    }
}
