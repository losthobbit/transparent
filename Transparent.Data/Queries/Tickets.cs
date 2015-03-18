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
    /// This must be transient, because injecting IUserContext is not safe.
    /// I might also want to consider creating the context at the start of a transaction in case I save an interrupted transaction.
    /// See http://stackoverflow.com/questions/10585478/one-dbcontext-per-web-request-why
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

        public void Create(Ticket ticket, int userId)
        {
            ticket.FkUserId = userId;
            ticket.CreatedDate = DateTime.UtcNow;
            ticket.ModifiedDate = DateTime.UtcNow;
            db.Tickets.Add(ticket);
            db.SaveChanges();
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
                    AddPoints(userPoint, userTag, -configuration.PointsToDeductWhenStartingTest);
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
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkTestId == testId && point.FkUserId == userId);
            if (userPoint == null)
                throw new NotSupportedException("The test has not started.  It cannot be answered.");
            if (userPoint.Answer != null)
                throw new NotSupportedException("The test has already been answered.  It cannot be answered again.");
            userPoint.Answer = answer;
            db.SaveChanges();
        }

        /// <summary>
        /// Gets tags that the user is competent in.
        /// </summary>
        /// <remarks>
        /// For performance purposes, GetExpertTags and GetCompetentTags could be combined into one DB call.
        /// </remarks>
        public IQueryable<Tag> GetCompetentTags(int userId)
        {
            return db.UserTags
            .Where(userTag => userTag.FkUserId == userId && userTag.TotalPoints >= configuration.PointsRequiredToBeCompetent)
            .Select(userTag => userTag.Tag);
        }

        /// <summary>
        /// Gets tags that the user is an expert in.
        /// </summary>
        /// <remarks>
        /// For performance purposes, GetExpertTags and GetCompetentTags could be combined into one DB call.
        /// </remarks>
        public IQueryable<Tag> GetExpertTags(int userId)
        {
            return db.UserTags
            .Where(userTag => userTag.FkUserId == userId && userTag.TotalPoints >= configuration.PointsRequiredToBeAnExpert)
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
            db.SaveChanges();
        }

        public Ticket FindTicket(int id)
        {
            return db.Tickets.Find(id);
        }

        private TicketTagViewModel GetTicketTagInfo(TicketTag ticketTag, int ticketUserId, int userId,
            IEnumerable<Tag> competentTags, IEnumerable<Tag> expertTags = null)
        {
            var info = new TicketTagViewModel
            {
                TagId = ticketTag.FkTagId
            };

            info.UserMayDelete = !ticketTag.Verified &&
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

        public IEnumerable<TicketTagViewModel> GetTicketTagInfoList(Ticket ticket, int userId)
        {
            var competentTags = GetCompetentTags(userId).ToList();
            var expertTags = GetExpertTags(userId).ToList();
            return ticket.TicketTags.Select(ticketTag => 
                GetTicketTagInfo(ticketTag, ticket.FkUserId, userId, competentTags, expertTags)).ToList();
        }

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        public void DeleteTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            db.TicketTags.Remove(GetDeleteableTicketTag(ticketId, tagId, userId));
            SetModifiedDate(ticketId);
            db.SaveChanges();
        }

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        public void VerifyTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            GetVerifyableTicketTag(ticketId, tagId, userId).Verified = true;
            SetModifiedDate(ticketId);
            db.SaveChanges();
        }

        /// <exception cref="NotSupportedException">User may not add tag.</exception>
        public void AddTicketTag(int ticketId, int tagId, int userId)
        {
            // TODO: Ensure the state of the ticket is TicketState.Verification
            if (!GetCompetentTags(userId).Any(tag => tag.Id == tagId))
                throw new NotSupportedException("User may not add tag");

            db.TicketTags.Add(new TicketTag { FkTicketId = ticketId, FkTagId = tagId, FkCreatedById = userId });
            SetModifiedDate(ticketId);

            db.SaveChanges();
        }

        /// <exception cref="NotSupportedException">User may not answer ticket.</exception>
        public void SetArgument(int ticketId, int userId, string argument)
        {
            if (db.Tickets.Single(ticket => ticket.Id == ticketId).State != TicketState.Discussion)
                throw new NotSupportedException("Argument cannot be set.  Ticket is not in the Discussion state.");

            var ticketTagIds = db.TicketTags.Where(tag => tag.FkTicketId == ticketId).Select(tag => tag.FkTagId);
            if(!GetExpertTags(userId).Select(tag => tag.Id).Intersect(ticketTagIds).Any())
                throw new NotSupportedException("User is not an expert in any of the ticket tags and cannot answer the ticket.");

            var argumentRow = db.Arguments.SingleOrDefault(arg => arg.FkTicketId == ticketId
                && arg.FkUserId == userId);
            if (argumentRow == null)
            {
                db.Arguments.Add(new Argument { FkTicketId = ticketId, FkUserId = userId, Body = argument });
            }
            else
            {
                argumentRow.Body = argument;
            }
            SetModifiedDate(ticketId);

            db.SaveChanges();
        }

        // TODO: Consider putting these in a partial class?
        #region Progress tickets

        /// <summary>
        /// Progresses tickets which are in the Verification state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        public void ProgressTicketsWithVerifiedTags()
        {
            var lastModified = DateTime.UtcNow - configuration.DelayAfterValidatingTags;
            var verifiedTickets = from ticket in db.Tickets
                                  where ticket.State == TicketState.Verification &&
                                  ticket.ModifiedDate <= lastModified && 
                                  ticket.TicketTags.Any() &&
                                  ticket.TicketTags.All(tag => tag.Verified)
                                  select ticket;

            foreach (var ticket in verifiedTickets)
            {
                SetNextState(ticket);
            }
            db.SaveChanges();
        }

        #endregion Progress tickets

        private void SetNextState(Ticket ticket)
        {
            var state = ticket.NextState;
            if (state == null)
                throw new NotSupportedException("Ticket does not have a next state");
            ticket.State = ticket.NextState.Value;
            ticket.ModifiedDate = DateTime.UtcNow;
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
                AddPoints
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
            AddPoints(testAnswer.TestMarkings
                .Where(testMarking => testPassed == null || testMarking.Passed != testPassed.Value)
                .Select(marking => marking.FkUserId), testAnswer.FkTagId, testAnswer.FkTestId, -configuration.PointsMarkersLoseForDisagreeingATestResult, PointReason.MarkedTest);

            // Increase markers' points in the case of a majority
            AddPoints(testAnswer.TestMarkings
                .Where(testMarking => testPassed.HasValue && testMarking.Passed == testPassed.Value)
                .Select(marking => marking.FkUserId), testAnswer.FkTagId, testAnswer.FkTestId, configuration.PointsMarkersGainForAgreeingATestResult, PointReason.MarkedTest);
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        private void AddPoints(UserPoint userPoint, UserTag userTag, int points)
        {
            userPoint.Quantity += points;
            userTag.TotalPoints += points;
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        private void AddPoints(int userId, int tagId, int testId, int points, PointReason reason)
        {
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkUserId == userId && point.FkTagId == tagId && point.FkTestId == testId);
            if (userPoint == null)
            {
                userPoint = new UserPoint { FkUserId = userId, FkTagId = tagId, FkTestId = testId, Reason = reason };
                db.UserPoints.Add(userPoint);
            }

            var userTag = db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
            if (userTag == null)
            {
                userTag = new UserTag { FkTagId = tagId, FkUserId = userId };
                db.UserTags.Add(userTag);
            }
            AddPoints(userPoint, userTag, points);
        }

        /// <summary>
        /// Adds points to the UserPoints and UserTags.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        private void AddPoints(IEnumerable<int> userId, int tagId, int testId, int points, PointReason reason)
        {
            foreach (var user in userId)
            {
                AddPoints(user, tagId, testId, points, reason);
            }
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
    }
}
