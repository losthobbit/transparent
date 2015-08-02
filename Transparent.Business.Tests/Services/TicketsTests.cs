﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Business.Services;
using Transparent.Data.Tests.Helpers;
using Common;
using NUnit.Framework;
using System.Security;
using Tests.Common;
using Ploeh.AutoFixture;
using Transparent.Business.ViewModels;
using Moq;
using Transparent.Data.Services;
using Transparent.Business.Interfaces;

namespace Transparent.Business.Tests.Services
{
    [TestFixture]
    public class TicketsTests : BaseTests
    {
        private Tickets target;

        private Mock<IDataService> mockDataService;
        private Mock<IUser> mockUserService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mockDataService = new Mock<IDataService>();
            mockUserService = new Mock<IUser>();

            target = new Tickets(UsersContext, mockDataService.Object, TestConfiguration, mockUserService.Object);
        }
        
        /// <summary>
        /// This is simply to avoid having to fix tests after moving code into the data service.
        /// </summary>
        /// <remarks>
        /// Feel free to remove this and do tests in the dataservice.
        /// </remarks>
        private void UseRealDataService()
        {
            var dataService = new DataService(MockTags.Object);
            mockDataService.Setup(x => x.AddPoints(It.IsAny<UserPoint>(), It.IsAny<UserTag>(), It.IsAny<int>()))
                .Callback<UserPoint, UserTag, int>(dataService.AddPoints);
            mockDataService.Setup(x => x.AddPoints(It.IsAny<IUsersContext>(), It.IsAny<IEnumerable<int>>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<PointReason>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Callback<IUsersContext, IEnumerable<int>, int, int, PointReason, int?, int?>(dataService.AddPoints);
        }

        #region MyQueue

        private void ArrangeMyQueue()
        {
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints;
        }

        [Test]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_more_than_or_equal_minimum_points_returns_ticket()
        {
            // Arrange
            ArrangeMyQueue();

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesCriticalThinkingSuggestion);
        }

        [Test]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_less_than_minimum_points_does_not_return_ticket()
        {
            // Arrange
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints - 1;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesCriticalThinkingSuggestion));
        }

        [Test]
        public void MyQueue_with_ticket_and_user_without_same_tag_does_not_return_ticket()
        {
            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesScubaDivingSuggestion));
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void MyQueue_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeMyQueue();
            TestData.JoesCriticalThinkingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesCriticalThinkingSuggestion);
        }

        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        public void MyQueue_returns_tickets_in_accepted_or_in_progress_state_that_are_assigned_to_me(TicketState ticketState)
        {
            // Arrange
            ArrangeMyQueue();
            TestData.JoesCriticalThinkingSuggestion.State = ticketState;
            TestData.JoesCriticalThinkingSuggestion.FkAssignedUserId = TestData.Stephen.UserId;
            TestData.JoesCriticalThinkingSuggestion.AssignedUser = TestData.Stephen;
            TestData.JoesCriticalThinkingSuggestion.History = new List<TicketHistory>
            {
                new TicketHistory { Id = 1, User = TestData.Joe, FkUserId = TestData.Joe.UserId },
                new TicketHistory { Id = 2, User = TestData.Stephen, FkUserId = TestData.Stephen.UserId },
            };

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesCriticalThinkingSuggestion);
        }

        [TestCase(TicketState.Accepted, true)]
        [TestCase(TicketState.InProgress, true)]
        [TestCase(TicketState.Accepted, false)]
        public void MyQueue_does_not_return_tickets_in_accepted_or_in_progress_state_that_are_not_assigned_to_me(TicketState ticketState,
            bool anyHistory)
        {
            // Arrange
            ArrangeMyQueue();
            TestData.JoesCriticalThinkingSuggestion.State = ticketState;
            if (anyHistory)
            {
                TestData.JoesCriticalThinkingSuggestion.FkAssignedUserId = TestData.Joe.UserId;
                TestData.JoesCriticalThinkingSuggestion.AssignedUser = TestData.Joe;
            }
            TestData.JoesCriticalThinkingSuggestion.History = anyHistory ? new List<TicketHistory>
            {
                new TicketHistory { Id = 2, User = TestData.Joe, FkUserId = TestData.Joe.UserId },
                new TicketHistory { Id = 1, User = TestData.Stephen, FkUserId = TestData.Stephen.UserId },
            } : new List<TicketHistory>();

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesCriticalThinkingSuggestion));
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Completed)]
        public void MyQueue_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeMyQueue();
            TestData.JoesCriticalThinkingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), TestData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesCriticalThinkingSuggestion));
        }

        #endregion MyQueue

        #region HighestRanked

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void HighestRanked_with_publicOnly_true_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.HighestRanked(new TicketsContainer(), true);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Completed)]
        public void HighestRanked_returns_tickets_in_public_and_non_public_states(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.HighestRanked(new TicketsContainer());

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Completed)]
        public void HighestRanked_with_publicOnly_true_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.HighestRanked(new TicketsContainer(), true);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesScubaDivingSuggestion));
        }

        #endregion HighestRanked

        #region Newest

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void Newest_with_publicOnly_true_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer(), true);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Completed)]
        public void Newest_returns_tickets_in_public_and_non_public_states(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer());

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == TestData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Completed)]
        public void Newest_with_publicOnly_true_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            TestData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer(), true);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == TestData.JoesScubaDivingSuggestion));
        }

        [TestCase(TicketType.Suggestion, TicketState.Verification)]
        [TestCase(TicketType.Suggestion, TicketState.Accepted)]
        [TestCase(TicketType.Question, TicketState.Discussion)]
        public void Newest_only_returns_tickets_with_the_specified_type_and_state(TicketType type, TicketState state)
        {
            // Arrange

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer { TicketType = type, TicketState = state });

            // Assert
            Assert.IsTrue(ticketsContainer.PagedList.All(ticket => ticket.TicketType == type && ticket.State == state));
        }

        #endregion Newest

        #region Answered

        [Test]
        public void Answered_only_returns_questions()
        {
            // Arrange
            TestData.StephensCriticalThinkingQuestion.State = TicketState.Completed;

            // Act
            var ticketsContainer = target.Answered(new TicketsContainer());

            // Assert
            Assert.IsTrue(ticketsContainer.PagedList.All(ticket => ticket is Question));
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        public void Answered_does_not_return_questions_in_non_completed_state(TicketState ticketState)
        {
            // Arrange
            TestData.StephensCriticalThinkingQuestion.State = ticketState;

            // Act
            var ticketsContainer = target.Answered(new TicketsContainer());

            // Assert
            Assert.IsTrue(ticketsContainer.PagedList.All(ticket => ticket.State == TicketState.Completed));
        }

        #endregion Answered

        #region Search

        private Search searchFilter;

        private void ArrangeSearch()
        {
            searchFilter = new Search
            {
                SearchString = "your"
            };
        }

        [Test]
        public void Search_with_search_term_in_body_returns_ticket()
        {
            //Arrange
            ArrangeSearch();

            //Act
            var response = target.Search(searchFilter);

            //Assert
            response.PagedList.Single(ticket => ticket == TestData.CriticalThinkingTestThatIsInTheVotingStage);
        }

        [Test]
        public void Search_without_search_term_in_body_does_not_return_ticket()
        {
            //Arrange
            var filter = new Search
            {
                SearchString = "supercalafragilistic"
            };

            //Act
            var response = target.Search(filter);

            //Assert
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == TestData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void Search_returns_tests_in_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            TestData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            response.PagedList.Single(ticket => ticket == TestData.CriticalThinkingTestThatJoeTookThatStephenMarked);
        }

        [TestCase(TicketState.Completed)]
        public void Search_does_not_return_tests_which_have_been_completed(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            TestData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == TestData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [TestCase(TicketState.Draft)]
        public void Search_does_not_return_tests_which_are_in_draft_state(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            TestData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == TestData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        #endregion Search

        #region GetUntakenTests

        private void ArrangeGetUntakenTests(bool competentInParents = true)
        {
            mockUserService.Setup(x => x.GetIncompetentParentsTags(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(competentInParents ? new List<Tag>() : new List<Tag> { TestData.CriticalThinkingTag });
        }

        [Test]
        public void GetUntakenTests_returns_only_tests_that_match_the_tag()
        {
            // Arrange
            ArrangeGetUntakenTests();
            var tag = TestData.CriticalThinkingTag;
            var userId = TestData.Stephen.UserId;

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => test.TicketTags.Single().Tag == tag));
        }

        [Test]
        public void GetUntakenTests_returns_only_tests_that_are_in_the_completed_state()
        {
            // Arrange
            ArrangeGetUntakenTests();
            var tag = TestData.CriticalThinkingTag;
            var userId = TestData.Stephen.UserId;

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.All(test => test.State == TicketState.Completed));
        }

        [Test]
        public void GetUntakenTests_returns_tests_that_have_not_been_taken_by_the_user()
        {
            // Arrange
            ArrangeGetUntakenTests();
            var tag = TestData.CriticalThinkingTag;
            var userId = TestData.Stephen.UserId;
            var stephensPoints = TestData.UsersContext.UserPoints.Where(userPoints => userPoints.User == TestData.Stephen);
            var testsStephenTook = stephensPoints
                .Where(point => point.Reason == PointReason.TookTest)
                .Select(point => point.TestTaken)
                .Where(test => test != null).ToList();
            Assert.IsTrue(testsStephenTook.Any());

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => !testsStephenTook.Contains(test)));
        }

        [Test]
        public void GetUntakenTests_returns_tests_that_were_marked_by_the_user()
        {
            // Arrange
            ArrangeGetUntakenTests();
            var tag = TestData.CriticalThinkingTag;
            var userId = TestData.Stephen.UserId;
            var stephensPoints = TestData.UsersContext.UserPoints.Where(userPoints => userPoints.User == TestData.Stephen);
            var testsStephenMarked = stephensPoints
                .Where(point => point.Reason == PointReason.MarkedTest)
                .Select(point => point.TestTaken)
                .Where(test => test != null).ToList();
            Assert.IsTrue(testsStephenMarked.Any());

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.Intersect(testsStephenMarked).Any());
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void GetUntakenTests_where_the_user_is_incompetent_in_the_parent_tag_throws_NotSupportedException()
        {
            // Arrange
            ArrangeGetUntakenTests(false);
            var tag = TestData.BungeeJumpingTag;
            var userId = TestData.Stephen.UserId;

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);
        }

        #endregion GetUntakenTest

        #region AnswerTest

        private int answerTest_TestId;
        private int answerTest_UserId;
        private string answerTest_Answer;

        private void ArrangeAnswerTest()
        {
            answerTest_TestId = TestData.CriticalThinkingTestThatJoeStarted.Id;
            answerTest_UserId = TestData.Joe.UserId;
            answerTest_Answer = Fixture.Create<string>();
        }

        [Test]
        public void AnswerTest_with_valid_parameters_sets_answer()
        {
            //Arrange
            ArrangeAnswerTest();
            var postSaveAssert = UsersContext.PostSaveAssert(() =>
                answerTest_Answer == TestData.UsersContext.UserPoints.SingleOrDefault(point => 
                    point.FkTestId == answerTest_TestId && point.FkUserId == answerTest_UserId).Answer);

            //Act
            target.AnswerTest(answerTest_TestId, answerTest_Answer, answerTest_UserId);

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void AnswerTest_without_badge_adds_badge()
        {
            //Arrange
            ArrangeAnswerTest();
            var postSaveAssert = UsersContext.PostSaveAssert(() => TestData.Joe.HasBadge(Badge.FirstTestAnswered));

            //Act
            target.AnswerTest(answerTest_TestId, answerTest_Answer, answerTest_UserId);

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void AnswerTest_without_badge_adds_badge_points()
        {
            //Arrange
            ArrangeAnswerTest();
            var postSaveAssert = UsersContext.PostSaveAssert(() =>
                mockDataService.Verify(x => x.AddApplicationPoints(UsersContext, TestData.Joe.UserId,
                    TestConfiguration.DiPointsForFirstBadge, Badge.FirstTestAnswered, null), Times.Once())
            );

            //Act
            target.AnswerTest(answerTest_TestId, answerTest_Answer, answerTest_UserId);

            //Assert
            postSaveAssert.Try();
        }

        #endregion AnswerTest

        #region TestsToBeMarked

        [Test]
        public void TestsToBeMarked_returns_tests()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.Any());
            Assert.IsTrue(actual.PagedList.All(item => item != null));
            Assert.IsTrue(actual.PagedList.All(item => item.Test != null));
        }

        [Test]
        public void TestsToBeMarked_does_not_return_tests_answered_by_userName()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != TestData.CriticalThinkingTestThatStephenTook));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_they_have_not_marked()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != TestData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_have_not_been_completely_marked()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != TestData.CriticalThinkingTestThatJoeTookThatHasBeenMarkedCompletely));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_have_been_answered_completely()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != TestData.CriticalThinkingTestThatJoeStarted));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_for_which_the_user_has_sufficient_points()
        {
            //Arrange
            foreach (var userTag in TestData.Stephen.Tags)
            {
                if (userTag.Tag == TestData.BungeeJumpingTag)
                {
                    userTag.TotalPoints = userTag.Tag.CompetentPoints - 1;
                }
            }

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != TestData.BungeeJumpingTestThatJoeTook));
        }

        #endregion

        #region TestToBeMarked

        [Test]
        public void TestToBeMarked_returns_tests_which_are_returned_by_TestsToBeMarked()
        {
            //Arrange
            var testList = target.GetTestsToBeMarked(new AnsweredTests(), TestData.Stephen.UserId);

            foreach (var test in testList.PagedList)
            {
                //Act
                var actual = target.GetTestToBeMarked(test.Id, TestData.Stephen.UserId);

                //Assert
                Assert.AreEqual(test.Id, actual.Id);
                Assert.AreEqual(test.Answer, actual.Answer);
                Assert.AreEqual(test.Test.Body, actual.Test.Body);
            }
        }

        [Test]
        public void TestToBeMarked_with_test_not_in_the_list_of_tests_to_be_marked_throws_security_exception()
        {
            //Arrange
            var invalidTests = new[]
            {
                TestData.CriticalThinkingTestThatStephenTook,
                TestData.CriticalThinkingTestThatJoeTookThatStephenMarked,
                TestData.CriticalThinkingTestThatJoeTookThatHasBeenMarkedCompletely,
                TestData.CriticalThinkingTestThatJoeStarted,
                TestData.BungeeJumpingTestThatJoeTook
            };

            foreach (var test in invalidTests)
            {
                try
                {
                    //Act
                    target.GetTestToBeMarked(test.Id, TestData.Stephen.UserId);

                    //Assert
                    Assert.Fail("SecurityException expected.");
                }
                catch (SecurityException)
                {
                    // Success
                }
            }
        }

        #endregion TestToBeMarked

        #region MarkTest

        UserPoint markTest_UserPoint;
        List<UserProfile> markTest_Markers;
        List<UserTag> markTest_Markers_UserTags;
        List<UserTag> markTest_FailMarkers_UserTags;
        List<UserTag> markTest_PassMarkers_UserTags;

        private void ArrangeMarkTest(int markersRequiredPerTest = 2, int timesTestMarked = 1, int? fails = null)
        {
            UseRealDataService();

            markTest_Markers = new List<UserProfile>();
            markTest_Markers_UserTags = new List<UserTag>();
            markTest_FailMarkers_UserTags = new List<UserTag>();
            markTest_PassMarkers_UserTags = new List<UserTag>();

            TestConfiguration.MarkersRequiredPerTest = markersRequiredPerTest;

            markTest_UserPoint = TestData.PointForCriticalThinkingTestThatJoeTookThatStephenMarked;
            var adminUserTag = TestData.AddUserTag(TestData.Admin, TestData.CriticalThinkingTag, TestData.CriticalThinkingTag.CompetentPoints);
            markTest_Markers_UserTags.Add(adminUserTag);
            markTest_PassMarkers_UserTags.Add(adminUserTag);
            var testsMarked = 0;
            while (markTest_UserPoint.TestMarkings.Count() < timesTestMarked)
            {
                bool pass = fails == null ? Fixture.Create<bool>() : testsMarked >= fails.Value;
                var user = TestData.CreateUser();
                var userTag = TestData.AddUserTag(user, TestData.CriticalThinkingTag, TestData.CriticalThinkingTag.CompetentPoints);
                markTest_Markers_UserTags.Add(userTag);
                if (pass)
                    markTest_PassMarkers_UserTags.Add(userTag);
                else
                    markTest_FailMarkers_UserTags.Add(userTag);
                target.MarkTest(markTest_UserPoint.Id, pass, user.UserId);
                markTest_Markers.Add(user);
                testsMarked++;
            }
        }

        [TestCase(3)]
        [TestCase(4)]
        public void MarkTest_with_less_than_all_markers_leaves_MarkingComplete_as_false(int markersRequiredPerTest)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 2);

            //Act
            target.MarkTest(markTest_UserPoint.Id, Fixture.Create<bool>(), TestData.Admin.UserId);

            //Assert
            Assert.IsFalse(markTest_UserPoint.MarkingComplete);
        }

        [Test]
        public void MarkTest_without_badge_adds_badge()
        {
            //Arrange
            ArrangeMarkTest();
            var postSaveAssert = UsersContext.PostSaveAssert(() => TestData.Admin.HasBadge(Badge.FirstTestMarked));

            //Act
            target.MarkTest(markTest_UserPoint.Id, Fixture.Create<bool>(), TestData.Admin.UserId);

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void MarkTest_without_badge_adds_badge_points()
        {
            //Arrange
            ArrangeMarkTest();
            var postSaveAssert = UsersContext.PostSaveAssert(() =>
                mockDataService.Verify(x => x.AddApplicationPoints(UsersContext, TestData.Admin.UserId,
                    TestConfiguration.DiPointsForFirstBadge, Badge.FirstTestMarked, null), Times.Once())
            );

            //Act
            target.MarkTest(markTest_UserPoint.Id, Fixture.Create<bool>(), TestData.Admin.UserId);

            //Assert
            postSaveAssert.Try();
        }

        [TestCase(3)]
        [TestCase(4)]
        public void MarkTest_with_all_markers_sets_MarkingComplete_to_true(int markersRequiredPerTest)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1);
            var actualMarkingComplete = markTest_UserPoint.MarkingComplete;
            UsersContext.SavedChanges += context =>
            {
                actualMarkingComplete = markTest_UserPoint.MarkingComplete;
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, Fixture.Create<bool>(), TestData.Admin.UserId);

            //Assert
            Assert.IsTrue(actualMarkingComplete);
        }

        [TestCase(4)]
        public void MarkTest_with_less_than_all_markers_leaves_markers_points_unchanged(int markersRequiredPerTest)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 2, markersRequiredPerTest / 2);
            var markersTagExpectedPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints).ToList();

            List<int> actualPoints = null;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(markersTagExpectedPoints, actualPoints);
        }

        [TestCase(4, 5)]
        public void MarkTest_with_all_markers_and_50_50_split_reduces_all_markers_points(int markersRequiredPerTest, int pointsToLose)
        {
            //Arrange
            TestConfiguration.PointsMarkersLoseForDisagreeingATestResult = pointsToLose;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, markersRequiredPerTest / 2);
            var markersTagExpectedPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints - pointsToLose).ToList();

            List<int> actualPoints = null;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(markersTagExpectedPoints, actualPoints);
        }

        [TestCase(5, 5, 3)]
        public void MarkTest_with_all_markers_and_non_50_50_split_reduces_minority_markers_points(int markersRequiredPerTest, int pointsToLose, int numberOfFails)
        {
            //Arrange
            TestConfiguration.PointsMarkersLoseForDisagreeingATestResult = pointsToLose;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);
            var minorityExpectedPoints = markTest_PassMarkers_UserTags.Select(tag => tag.TotalPoints - pointsToLose).ToList();

            List<int> actualPoints = null;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_PassMarkers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(minorityExpectedPoints, actualPoints);
        }

        [TestCase(5, 7, 3)]
        public void MarkTest_with_all_markers_and_non_50_50_split_increases_majority_markers_points(int markersRequiredPerTest, int pointsToGain, int numberOfFails)
        {
            //Arrange
            TestConfiguration.PointsMarkersGainForAgreeingATestResult = pointsToGain;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);
            var majorityExpectedPoints = markTest_FailMarkers_UserTags.Select(tag => tag.TotalPoints + pointsToGain).ToList();

            List<int> actualPoints = null;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_FailMarkers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(majorityExpectedPoints, actualPoints);
        }

        [TestCase(5, 7, 2)]
        public void MarkTest_with_all_markers_and_majority_of_markers_passed_increases_learners_points(int markersRequiredPerTest, int pointsToGain, int numberOfFails)
        {
            //Arrange
            TestConfiguration.PointsForPassingATest = pointsToGain;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var actualPoints = markTest_UserPoint.Quantity;
            var totalPoints = TestData.JoesCriticalThinkingTag.TotalPoints;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_UserPoint.Quantity;
                totalPoints = TestData.JoesCriticalThinkingTag.TotalPoints;
            };
            var expectedPoints = pointsToGain;
            // e.g. if total = 10, actualPoints = -2, and pointsToGain = 7, then expected total = 19
            var expectedTotalPoints = totalPoints + pointsToGain - actualPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            Assert.AreEqual(expectedPoints, actualPoints);
            Assert.AreEqual(expectedTotalPoints, totalPoints);
        }

        [TestCase(5, 3)]
        public void MarkTest_with_all_markers_and_majority_of_markers_failed_doesnt_change_learners_points(int markersRequiredPerTest, int numberOfFails)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var expectedPoints = markTest_UserPoint.Quantity;
            var expectedTotalPoints = TestData.JoesCriticalThinkingTag.TotalPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            Assert.AreEqual(expectedPoints, markTest_UserPoint.Quantity);
            Assert.AreEqual(expectedTotalPoints, TestData.JoesCriticalThinkingTag.TotalPoints);
        }

        [TestCase(6, 3)]
        public void MarkTest_with_all_markers_and_50_50_split_resets_learners_points(int markersRequiredPerTest, int numberOfFails)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var actualPoints = markTest_UserPoint.Quantity;
            var totalPoints = TestData.JoesCriticalThinkingTag.TotalPoints;
            UsersContext.SavedChanges += context =>
            {
                actualPoints = markTest_UserPoint.Quantity;
                totalPoints = TestData.JoesCriticalThinkingTag.TotalPoints;
            };
            var expectedPoints = 0;
            // e.g. if total = 10, and actualPoints = -2, then expected total = 12
            var expectedTotalPoints = totalPoints - actualPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, TestData.Admin.UserId);

            //Assert
            Assert.AreEqual(expectedPoints, actualPoints);
            Assert.AreEqual(expectedTotalPoints, totalPoints);
        }

        #endregion MarkTest

        #region GetTicketTagInfoList

        private Ticket getTicketTagInfoList_Ticket;

        private void ArrangeGetTicketInfoTagList
        (
            Relative userPointsForTag = Relative.GreaterThan,
            KnowledgeLevel knowledgeLevel = KnowledgeLevel.Competent
        )
        {
            TestData.StephensCriticalThinkingTag.TotalPoints = 
                (knowledgeLevel == KnowledgeLevel.Competent
                ? TestData.CriticalThinkingTag.CompetentPoints
                : TestData.CriticalThinkingTag.ExpertPoints)
                + (int)userPointsForTag;

            getTicketTagInfoList_Ticket = new Suggestion
            {
                User = TestData.Joe,
                FkUserId = TestData.Joe.UserId,
                TicketTags = new List<TicketTag>
                {
                    new TicketTag
                    {
                        Verified = false,
                        CreatedBy = TestData.Joe,
                        FkCreatedById = TestData.Joe.UserId,
                        Tag = TestData.CriticalThinkingTag,
                        FkTagId = TestData.CriticalThinkingTag.Id
                    }
                }
            };
        }

        [TestCase(Relative.GreaterThan)]
        [TestCase(Relative.EqualTo)]
        public void GetTicketTagInfoList_not_verified_and_created_by_someone_else_and_user_has_competent_or_more_points_for_that_tag_returns_UserCanValidate_is_true(Relative userPointsForTag)
        {
            //Arrange
            ArrangeGetTicketInfoTagList(userPointsForTag);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.AreEqual(getTicketTagInfoList_Ticket.TicketTags.Single().FkTagId, actual.TagId);
            Assert.IsTrue(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_not_verified_and_created_by_null_and_tickets_user_is_not_user_and_user_has_competent_or_more_points_for_that_tag_returns_UserCanValidate_is_true()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().CreatedBy = null;
            getTicketTagInfoList_Ticket.TicketTags.Single().FkCreatedById = null;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsTrue(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_verified_returns_UserMayVerify_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().Verified = true;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_created_by_user_returns_UserMayVerify_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().CreatedBy = TestData.Stephen;
            getTicketTagInfoList_Ticket.TicketTags.Single().FkCreatedById = TestData.Stephen.UserId;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_created_by_null_and_ticket_user_is_user_returns_UserCanValidate_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().CreatedBy = null;
            getTicketTagInfoList_Ticket.TicketTags.Single().FkCreatedById = null;
            getTicketTagInfoList_Ticket.User = TestData.Stephen;
            getTicketTagInfoList_Ticket.FkUserId = TestData.Stephen.UserId;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_user_has_less_than_competent_points_for_that_tag_returns_UserCanValidate_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList(Relative.LessThan);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_user_has_less_than_expert_points_for_that_tag_returns_UserIsExpert_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList(Relative.LessThan, KnowledgeLevel.Expert);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserIsExpert);
        }

        [TestCase(Relative.GreaterThan)]
        [TestCase(Relative.EqualTo)]
        public void GetTicketTagInfoList_user_has_expert_points_for_that_tag_returns_UserIsExpert_is_true(
            Relative userPointsForTag)
        {
            //Arrange
            ArrangeGetTicketInfoTagList(userPointsForTag, KnowledgeLevel.Expert);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, TestData.Stephen.UserId).Single();

            //Assert
            Assert.IsTrue(actual.UserIsExpert);
        }

        [Test]
        public void GetTicketTagInfoList_userId_is_minus_1_returns_UserMayDelete_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, -1).Single();

            //Assert
            Assert.IsFalse(actual.UserMayDelete);
        }

        [Test]
        public void GetTicketTagInfoList_userId_is_minus_1_returns_UserMayVerify_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().CreatedBy = null;
            getTicketTagInfoList_Ticket.TicketTags.Single().FkCreatedById = null;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, -1).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_userId_is_minus_1_returns_UserIsExpert_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, -1).Single();

            //Assert
            Assert.IsFalse(actual.UserIsExpert);
        }

        #endregion GetTicketTagInfoList

        #region StartTest

        private void ArrangeStartTest(bool competentInParents = true)
        {
            UseRealDataService();
            
            mockUserService.Setup(x => x.GetIncompetentParentsTags(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(competentInParents ? new List<Tag>() : new List<Tag> { TestData.CriticalThinkingTag });
        }
        
        [Test]
        public void StartTest_with_user_without_associated_tag_adds_tag_to_user()
        {
            //Arrange
            ArrangeStartTest();
            UserTag stephensScubaDivingTag = null;
            UsersContext.SavedChanges += context =>
            {
                stephensScubaDivingTag = context.UserTags.SingleOrDefault(tag => 
                    tag.FkTagId == TestData.ScubaDivingTag.Id &&
                    tag.FkUserId == TestData.Stephen.UserId);
            };

            //Act
            target.StartTest(TestData.ScubaDivingTestThatJoeTook, TestData.Stephen.UserId);

            //Assert
            Assert.IsNotNull(stephensScubaDivingTag);
        }

        [TestCase(1)]
        [TestCase(0)]
        public void StartTest_with_points_higher_than_or_equal_to_required_points_deducts_points(int pointsAboveRequired)
        {
            //Arrange
            ArrangeStartTest();
            TestData.StephensCriticalThinkingTag.TotalPoints = TestConfiguration.PointsRequiredBeforeDeductingPoints + pointsAboveRequired;
            int? actual = null;
            UsersContext.SavedChanges += context =>
            {
                var newPoint = context.UserPoints.Single(point =>
                    point.FkTagId == TestData.CriticalThinkingTag.Id &&
                    point.FkTestId == TestData.CriticalThinkingTestThatJoeTook.Id &&
                    point.User == TestData.Stephen);
                actual = newPoint.Quantity;
            };

            //Act
            target.StartTest(TestData.CriticalThinkingTestThatJoeTook, TestData.Stephen.UserId);

            //Assert
            Assert.AreEqual(-TestConfiguration.PointsToDeductWhenStartingTest, actual.Value);
        }

        [TestCase(1)]
        [TestCase(0)]
        public void StartTest_with_points_higher_than_or_equal_to_required_points_deducts_points_from_user_tag_total(int pointsAboveRequired)
        {
            //Arrange
            ArrangeStartTest();
            TestData.StephensCriticalThinkingTag.TotalPoints = TestConfiguration.PointsRequiredBeforeDeductingPoints + pointsAboveRequired;
            var prevPoints = TestData.StephensCriticalThinkingTag.TotalPoints;
            var actualTotalPoints = prevPoints;
            UsersContext.SavedChanges += context =>
            {
                actualTotalPoints = TestData.StephensCriticalThinkingTag.TotalPoints;
            };

            //Act
            target.StartTest(TestData.CriticalThinkingTestThatJoeTook, TestData.Stephen.UserId);

            //Assert
            Assert.AreEqual(prevPoints - TestConfiguration.PointsToDeductWhenStartingTest, actualTotalPoints);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void StartTest_with_points_less_than_or_equal_to_required_points_does_not_deduct_points(int pointsBelowRequired)
        {
            //Arrange
            ArrangeStartTest();
            TestData.StephensCriticalThinkingTag.TotalPoints = TestConfiguration.PointsRequiredBeforeDeductingPoints - pointsBelowRequired;

            //Act
            target.StartTest(TestData.CriticalThinkingTestThatJoeTook, TestData.Stephen.UserId);

            //Assert
            var newPoint = TestData.UsersContext.UserPoints.Single(point =>
                point.FkTagId == TestData.CriticalThinkingTag.Id &&
                point.FkTestId == TestData.CriticalThinkingTestThatJoeTook.Id &&
                point.User == TestData.Stephen);
            Assert.AreEqual(0, newPoint.Quantity);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void StartTest_with_an_incompetent_parent_tag_throws_NotSupportedException()
        {
            //Arrange
            ArrangeStartTest(false);

            //Act
            target.StartTest(TestData.ScubaDivingTestThatJoeTook, TestData.Stephen.UserId);
        }

        #endregion StartTest

        #region Create

        private Ticket createTicket;

        private void ArrangeCreate()
        {
            createTicket = new Question
            {
            };
        }

        [Test]
        public void Create_with_valid_parameters_sets_ModifiedDate()
        {
            //Arrange    
            ArrangeCreate();
            Ticket actualTicket = null;
            UsersContext.SavedChanges += context =>
            {
                actualTicket = context.Tickets.Last();
            };            

            //Act
            target.Create(createTicket, TestData.Stephen.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }
       
        [Test]
        public void Create_without_badge_adds_badge()
        {
            //Arrange
            ArrangeCreate();
            var postSaveAssert = UsersContext.PostSaveAssert(() => TestData.Stephen.HasBadge(Badge.FirstTicketCreated));

            //Act
            target.Create(createTicket, TestData.Stephen.UserId);

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void Create_without_badge_adds_badge_points()
        {
            //Arrange
            ArrangeCreate();
            var postSaveAssert = UsersContext.PostSaveAssert(() =>
                mockDataService.Verify(x => x.AddApplicationPoints(UsersContext, TestData.Stephen.UserId, 
                    TestConfiguration.DiPointsForFirstBadge,Badge.FirstTicketCreated, createTicket.Id), Times.Once())
            );

            //Act
            target.Create(createTicket, TestData.Stephen.UserId);

            //Assert
            postSaveAssert.Try();
        }

        #endregion Create

        #region AddTicketTag

        [Test]
        public void AddTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            UsersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.AddTicketTag(ticket.Id, TestData.CriticalThinkingTag.Id, TestData.Stephen.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Draft)]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddTicketTag_with_ticket_not_in_verification_state_throws_NotSupportedException(TicketState state)
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            ticket.State = state;

            //Act
            target.AddTicketTag(ticket.Id, TestData.CriticalThinkingTag.Id, TestData.Stephen.UserId);
        }

        #endregion AddTicketTag

        #region DeleteTicketTag

        [Test]
        public void DeleteTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            UsersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.DeleteTicketTag(ticket.Id, TestData.ScubaDivingTag.Id, TestData.Admin.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.InProgress)]
        [ExpectedException(typeof(NotSupportedException))]
        public void DeleteTicketTag_with_ticket_not_in_verification_state_throws_NotSupportedException(TicketState state)
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            ticket.State = state;

            //Act
            target.DeleteTicketTag(ticket.Id, TestData.ScubaDivingTag.Id, TestData.Admin.UserId);
        }

        #endregion DeleteTicketTag

        #region VerifyTicketTag

        [Test]
        public void VerifyTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            UsersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.VerifyTicketTag(ticket.Id, TestData.ScubaDivingTag.Id, TestData.Admin.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        [TestCase(TicketState.Completed)]
        [TestCase(TicketState.Discussion)]
        [ExpectedException(typeof(NotSupportedException))]
        public void VerifyTicketTag_with_ticket_not_in_verification_state_throws_NotSupportedException(TicketState state)
        {
            //Arrange
            var ticket = TestData.JoesScubaDivingSuggestion;
            ticket.State = state;

            //Act
            target.VerifyTicketTag(ticket.Id, TestData.ScubaDivingTag.Id, TestData.Admin.UserId);
        }

        #endregion VerifyTicketTag

        #region SetArgument

        private Argument setArgument_ExistingArgument;
        private Ticket setArgument_Ticket;

        private void ArrangeSetArgument(bool existingArgument = false)
        {
            setArgument_Ticket = TestData.JoesScubaDivingSuggestion;
            setArgument_Ticket.State = TicketState.Discussion;

            if (existingArgument)
            {
                setArgument_ExistingArgument = new Argument
                {
                    FkTicketId = TestData.JoesScubaDivingSuggestion.Id,
                    FkUserId = TestData.Admin.UserId
                };
                TestData.UsersContext.Arguments.Add(setArgument_ExistingArgument);
            }
         }

        [Test]
        public void SetArgument_with_valid_user_and_no_argument_adds_argument()
        {
            //Arrange
            ArrangeSetArgument();
            Argument newArgument = null;
            UsersContext.SavedChanges += context =>
            {
                newArgument = context.Arguments.Last();
            };           

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");

            //Assert
            Assert.AreEqual("hello", newArgument.Body);
            Assert.AreEqual(TestData.Admin.UserId, TestData.Admin.UserId);
            AssertModifiedDateSet(TestData.JoesScubaDivingSuggestion.ModifiedDate);
        }

        [Test]
        public void SetArgument_without_argument_and_badge_adds_badge()
        {
            //Arrange
            ArrangeSetArgument();
            var postSaveAssert = UsersContext.PostSaveAssert(() => TestData.Admin.HasBadge(Badge.FirstArgument));

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void SetArgument_without_argument_and_adds_badge_points()
        {
            //Arrange
            ArrangeSetArgument();
            var postSaveAssert = UsersContext.PostSaveAssert(() =>
                mockDataService.Verify(x => x.AddApplicationPoints(UsersContext, TestData.Admin.UserId,
                    TestConfiguration.DiPointsForFirstBadge, Badge.FirstArgument, setArgument_Ticket.Id), Times.Once())
            );

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");

            //Assert
            postSaveAssert.Try();
        }
      
        [Test]
        public void SetArgument_with_valid_user_and_existing_argument_updates_argument()
        {
            //Arrange
            ArrangeSetArgument(true);
            string actualArgument = null;
            UsersContext.SavedChanges += context =>
            {
                actualArgument = setArgument_ExistingArgument.Body;
            };

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");

            //Assert
            Assert.AreEqual("hello", actualArgument);
            AssertModifiedDateSet(TestData.JoesScubaDivingSuggestion.ModifiedDate);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetArgument_with_user_who_is_not_an_expert_for_the_ticket_throws_NotSupportedException()
        {
            //Arrange
            ArrangeSetArgument();
            TestData.AdminsScubaDivingTag.TotalPoints = TestData.ScubaDivingTag.ExpertPoints - 1;

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Completed)]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetArgument_with_ticket_not_in_Discussion_state_throws_NotSupportedException(TicketState state)
        {
            //Arrange
            ArrangeSetArgument();
            setArgument_Ticket.State = state;

            //Act
            target.SetArgument(setArgument_Ticket.Id, TestData.Admin.UserId, "hello");
        }

        #endregion SetArgument

        #region SetVote

        private Ticket setVote_Ticket;
        private bool setVoteCalled = false;
        private bool setVoteCalledAndSaved = false;

        private void ArrangeSetVote()
        {
            setVote_Ticket = TestData.JoesScubaDivingSuggestion;
            setVote_Ticket.State = TicketState.Voting;
            TestData.AdminsScubaDivingTag.TotalPoints = TestData.ScubaDivingTag.CompetentPoints + new Random().Next(3);
            mockDataService.Setup(x => x.SetVote(setVote_Ticket, It.IsAny<Stance>(), TestData.Admin.UserId))
                .Callback(() => setVoteCalled = true);
            UsersContext.SavedChanges += context => setVoteCalledAndSaved = setVoteCalled;
        }
        
        [Test]
        public void SetVote_with_valid_parameters_calls_SetVote_on_data_service()
        {
            //Arrange
            ArrangeSetVote();

            //Act
            target.SetVote(setVote_Ticket.Id, Fixture.Create<Stance>(), TestData.Admin.UserId);

            //Assert
            Assert.IsTrue(setVoteCalledAndSaved);
        }
       
        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetVote_with_ticket_not_in_Voting_state_throws_NotSupportedException(TicketState state)
        {
            //Arrange
            ArrangeSetVote();
            setVote_Ticket.State = state;

            //Act
            target.SetVote(setVote_Ticket.Id, Fixture.Create<Stance>(), TestData.Admin.UserId);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetVote_with_user_who_is_not_a_competent_for_the_ticket_throws_NotSupportedException()
        {
            //Arrange
            ArrangeSetVote();
            TestData.AdminsScubaDivingTag.TotalPoints = TestData.ScubaDivingTag.CompetentPoints - 1;

            //Act
            target.SetVote(setVote_Ticket.Id, Fixture.Create<Stance>(), TestData.Admin.UserId);
        }

        #endregion SetVote

        #region Assign

        private AssignViewModel assignViewModel;

        private void ArrangeAssign(TicketState state = TicketState.Completed)
        {
            TestData.JoesCriticalThinkingSuggestion.State = TicketState.InProgress;
            assignViewModel = new AssignViewModel
            {
                TicketId = TestData.JoesCriticalThinkingSuggestion.Id,
                TicketState = state,
                Username = null
            };
        }

        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Completed)]
        public void Assign_with_valid_parameters_sets_state(TicketState state)
        {
            //Arrange
            ArrangeAssign(state);
            var actualState = TestData.JoesCriticalThinkingSuggestion.State;
            TestData.UsersContext.SavedChanges += context => actualState = TestData.JoesCriticalThinkingSuggestion.State;

            //Act
            target.Assign(assignViewModel, TestData.Stephen.UserId);

            //Assert
            Assert.AreEqual(state, actualState);
        }

        [TestCase(TicketType.Question)]
        [TestCase(TicketType.Test)]
        [ExpectedException(typeof(NotSupportedException))]
        public void Assign_with_ticket_not_suggestion_throws_NotSupportedException(TicketType ticketType)
        {
            //Arrange
            ArrangeAssign();
            assignViewModel.TicketId = ticketType == TicketType.Question
                ? TestData.StephensCriticalThinkingQuestion.Id
                : TestData.CriticalThinkingTestThatJoeTook.Id;

            //Act
            target.Assign(assignViewModel, TestData.Stephen.UserId);
        }

        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Completed)]
        public void Assign_with_valid_parameters_sets_assigned_user(TicketState state)
        {
            //Arrange
            ArrangeAssign(state);
            var postSaveAssert = UsersContext.PostSaveAssert(() => 
                TestData.JoesCriticalThinkingSuggestion.FkAssignedUserId == TestData.Stephen.UserId);

            //Act
            target.Assign(assignViewModel, TestData.Stephen.UserId);

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [Test]
        public void Assign_with_valid_parameters_returns_username()
        {
            //Arrange
            ArrangeAssign();

            //Act
            var actual = target.Assign(assignViewModel, TestData.Stephen.UserId);

            //Assert
            Assert.AreEqual(TestData.Stephen.UserName, actual.Username);
        }
       
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.Completed)]
        public void Assign_with_valid_parameters_creates_history_with_state_and_userId(TicketState state)
        {
            //Arrange
            ArrangeAssign(state);
            TicketHistory actualHistory = null;
            TestData.UsersContext.SavedChanges += context => actualHistory =
                TestData.JoesCriticalThinkingSuggestion.History.Last();

            //Act
            target.Assign(assignViewModel, TestData.Stephen.UserId);

            //Assert
            AssertModifiedDateSet(actualHistory.Date);
            Assert.AreEqual(TestData.Stephen.UserId, actualHistory.FkUserId);
            Assert.AreEqual(state, actualHistory.State);
        }

        #endregion Assign

        #region GetCompetentTags

        [TestCase(1)]
        [TestCase(0)]
        public void GetCompetentTags_with_UserTags_higher_than_or_equal_to_competence_level_returns_tags(int higherBy)
        {
            //Arrange
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints + higherBy;

            //Act
            var tags = target.GetCompetentTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.CriticalThinkingTag);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetCompetentTags_with_UserTags_lower_than_competence_level_does_not_return_tags(int lowerBy)
        {
            //Arrange
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints - lowerBy;

            //Act
            var tags = target.GetCompetentTags(TestData.Stephen.UserId);

            //Assert
            Assert.IsFalse(tags.Contains(TestData.CriticalThinkingTag));
        }

        [Test]
        public void GetCompetentTags_without_UserTags_with_competence_level_of_0_returns_tags()
        {
            //Arrange
            TestData.ScubaDivingTag.CompetentPoints = 0;

            //Act
            var tags = target.GetCompetentTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.ScubaDivingTag);
        }

        [Test]
        public void GetCompetentTags_with_UserTags_with_competence_level_of_0_returns_single_instance_of_tag()
        {
            //Arrange
            TestData.CriticalThinkingTag.CompetentPoints = 0;

            //Act
            var tags = target.GetCompetentTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.CriticalThinkingTag);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetCompetentTags_without_UserTags_with_competence_level_greater_than_0_does_not_return_tags(int competentPoints)
        {
            //Arrange
            TestData.ScubaDivingTag.CompetentPoints = competentPoints;

            //Act
            var tags = target.GetCompetentTags(TestData.Stephen.UserId);

            //Assert
            Assert.IsFalse(tags.Contains(TestData.ScubaDivingTag));
        }
        
        [Test]
        public void GetCompetentTags_with_userId_equal_to_minus_1_returns_no_tags()
        {
            //Act
            var competentTags = target.GetCompetentTags(-1);

            //Assert
            Assert.IsFalse(competentTags.Any());
        }

        #endregion GetCompetentTags

        #region GetExpertTags

        [TestCase(1)]
        [TestCase(0)]
        public void GetExpertTags_with_UserTags_higher_than_or_equal_to_competence_level_returns_tags(int higherBy)
        {
            //Arrange
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.ExpertPoints + higherBy;

            //Act
            var tags = target.GetExpertTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.CriticalThinkingTag);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetExpertTags_with_UserTags_lower_than_ExpertPoints_level_does_not_return_tags(int lowerBy)
        {
            //Arrange
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.ExpertPoints - lowerBy;

            //Act
            var tags = target.GetExpertTags(TestData.Stephen.UserId);

            //Assert
            Assert.IsFalse(tags.Contains(TestData.CriticalThinkingTag));
        }

        [Test]
        public void GetExpertTags_without_UserTags_with_ExpertPoints_level_of_0_returns_tags()
        {
            //Arrange
            TestData.ScubaDivingTag.ExpertPoints = 0;

            //Act
            var tags = target.GetExpertTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.ScubaDivingTag);
        }

        [Test]
        public void GetExpertTags_with_UserTags_with_ExpertPoints_level_of_0_returns_single_instance_of_tag()
        {
            //Arrange
            TestData.CriticalThinkingTag.ExpertPoints = 0;

            //Act
            var tags = target.GetExpertTags(TestData.Stephen.UserId);

            //Assert
            tags.Single(t => t == TestData.CriticalThinkingTag);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetExpertTags_without_UserTags_with_competence_level_greater_than_0_does_not_return_tags(int expertPoints)
        {
            //Arrange
            TestData.ScubaDivingTag.ExpertPoints = expertPoints;

            //Act
            var tags = target.GetExpertTags(TestData.Stephen.UserId);

            //Assert
            Assert.IsFalse(tags.Contains(TestData.ScubaDivingTag));
        }

        [Test]
        public void GetExpertTags_with_userId_equal_to_minus_1_returns_no_tags()
        {
            //Act
            var expertTags = target.GetExpertTags(-1);

            //Assert
            Assert.IsFalse(expertTags.Any());
        }

        #endregion GetExpertTags

    }
}
