﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Business.Services;
using Transparent.Business.Tests.Helpers;
using Common;
using NUnit.Framework;
using System.Security;
using Tests.Common;
using Ploeh.AutoFixture;
using Transparent.Business.ViewModels;

namespace Transparent.Business.Tests.Services
{
    [TestFixture]
    public class TicketsTests
    {
        private Fixture fixture;

        private Tickets target;

        private TestData testData;
        private IConfiguration testConfiguration;
        private FakeUsersContext usersContext;

        [SetUp()]
        public void SetUp()
        {
            fixture = new Fixture();

            testData = TestData.Create();
            usersContext = testData.UsersContext;
            testConfiguration = new TestConfig();
            target = new Tickets(usersContext, testConfiguration);
        }

        #region MyQueue

        private void ArrangeMyQueue()
        {
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag;
        }

        [Test]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_more_than_or_equal_minimum_points_returns_ticket()
        {
            // Arrange
            ArrangeMyQueue();

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserId);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == testData.JoesCriticalThinkingSuggestion);
        }

        [Test]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_less_than_minimum_points_does_not_return_ticket()
        {
            // Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag - 1;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == testData.JoesCriticalThinkingSuggestion));
        }

        [Test]
        public void MyQueue_with_ticket_and_user_without_same_tag_does_not_return_ticket()
        {
            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == testData.JoesScubaDivingSuggestion));
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void MyQueue_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeMyQueue();
            testData.JoesCriticalThinkingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserId);

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == testData.JoesCriticalThinkingSuggestion);
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Completed)]
        public void MyQueue_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeMyQueue();
            testData.JoesCriticalThinkingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserId);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == testData.JoesCriticalThinkingSuggestion));
        }

        #endregion MyQueue

        #region HighestRanked

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void HighestRanked_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            testData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.HighestRanked(new TicketsContainer());

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == testData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Completed)]
        public void HighestRanked_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            testData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.HighestRanked(new TicketsContainer());

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == testData.JoesScubaDivingSuggestion));
        }

        #endregion HighestRanked

        #region Newest

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void Newest_returns_tickets_in_public_state(TicketState ticketState)
        {
            // Arrange
            testData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer());

            // Assert
            ticketsContainer.PagedList.Single(ticket => ticket == testData.JoesScubaDivingSuggestion);
        }

        [TestCase(TicketState.Draft)]
        [TestCase(TicketState.Rejected)]
        [TestCase(TicketState.Accepted)]
        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Completed)]
        public void Newest_does_not_return_tickets_in_non_public_state(TicketState ticketState)
        {
            // Arrange
            testData.JoesScubaDivingSuggestion.State = ticketState;

            // Act
            var ticketsContainer = target.Newest(new TicketsContainer());

            // Assert
            Assert.IsFalse(ticketsContainer.PagedList.Any(ticket => ticket == testData.JoesScubaDivingSuggestion));
        }

        #endregion Newest

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
            response.PagedList.Single(ticket => ticket == testData.CriticalThinkingTestThatJoeTookThatStephenMarked);
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
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == testData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [TestCase(TicketState.Verification)]
        [TestCase(TicketState.Discussion)]
        [TestCase(TicketState.Voting)]
        public void Search_returns_tests_in_public_state(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            testData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            response.PagedList.Single(ticket => ticket == testData.CriticalThinkingTestThatJoeTookThatStephenMarked);
        }

        [TestCase(TicketState.Completed)]
        public void Search_does_not_return_tests_which_have_been_completed(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            testData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == testData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [TestCase(TicketState.Draft)]
        public void Search_does_not_return_tests_which_are_in_draft_state(TicketState ticketState)
        {
            // Arrange
            ArrangeSearch();
            testData.CriticalThinkingTestThatJoeTookThatStephenMarked.State = ticketState;

            // Act
            var response = target.Search(searchFilter);

            // Assert
            Assert.IsFalse(response.PagedList.Any(ticket => ticket == testData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        #endregion Search

        #region GetUntakenTests

        [Test]
        public void GetUntakenTests_returns_only_tests_that_match_the_tag()
        {
            // Arrange
            var tag = testData.CriticalThinkingTag;
            var userId = testData.Stephen.UserId;

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => test.TicketTags.Single().Tag == tag));
        }

        [Test]
        public void GetUntakenTests_returns_tests_that_have_not_been_taken_by_the_user()
        {
            // Arrange
            var tag = testData.CriticalThinkingTag;
            var userId = testData.Stephen.UserId;
            var stephensPoints = testData.UsersContext.UserPoints.Where(userPoints => userPoints.User == testData.Stephen);
            var testsStephenTook = stephensPoints.Select(point => point.TestTaken).Where(test => test != null).ToList();
            Assert.IsTrue(testsStephenTook.Any());

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userId);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => !testsStephenTook.Contains(test)));
        }

        #endregion GetUntakenTest

        #region TestsToBeMarked

        [Test]
        public void TestsToBeMarked_returns_tests()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

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
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != testData.CriticalThinkingTestThatStephenTook));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_they_have_not_marked()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != testData.CriticalThinkingTestThatJoeTookThatStephenMarked));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_have_not_been_completely_marked()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != testData.CriticalThinkingTestThatJoeTookThatHasBeenMarkedCompletely));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_that_have_been_answered_completely()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != testData.CriticalThinkingTestThatJoeStarted));
        }

        [Test]
        public void TestsToBeMarked_returns_only_tests_for_which_the_user_has_sufficient_points()
        {
            //Arrange

            //Act
            var actual = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            //Assert
            Assert.IsTrue(actual.PagedList.All(item => item.Test != testData.BungeeJumpingTestThatJoeTook));
        }

        #endregion

        #region TestToBeMarked

        [Test]
        public void TestToBeMarked_returns_tests_which_are_returned_by_TestsToBeMarked()
        {
            //Arrange
            var testList = target.GetTestsToBeMarked(new AnsweredTests(), testData.Stephen.UserId);

            foreach (var test in testList.PagedList)
            {
                //Act
                var actual = target.GetTestToBeMarked(test.Id, testData.Stephen.UserId);

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
                testData.CriticalThinkingTestThatStephenTook,
                testData.CriticalThinkingTestThatJoeTookThatStephenMarked,
                testData.CriticalThinkingTestThatJoeTookThatHasBeenMarkedCompletely,
                testData.CriticalThinkingTestThatJoeStarted,
                testData.BungeeJumpingTestThatJoeTook
            };

            foreach (var test in invalidTests)
            {
                try
                {
                    //Act
                    target.GetTestToBeMarked(test.Id, testData.Stephen.UserId);

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
            markTest_Markers = new List<UserProfile>();
            markTest_Markers_UserTags = new List<UserTag>();
            markTest_FailMarkers_UserTags = new List<UserTag>();
            markTest_PassMarkers_UserTags = new List<UserTag>();

            testConfiguration.MarkersRequiredPerTest = markersRequiredPerTest;

            markTest_UserPoint = testData.PointForCriticalThinkingTestThatJoeTookThatStephenMarked;
            var adminUserTag = testData.AddUserTag(testData.Admin, testData.CriticalThinkingTag, testConfiguration.PointsRequiredToBeCompetent);
            markTest_Markers_UserTags.Add(adminUserTag);
            markTest_PassMarkers_UserTags.Add(adminUserTag);
            var testsMarked = 0;
            while (markTest_UserPoint.TestMarkings.Count() < timesTestMarked)
            {
                bool pass = fails == null ? fixture.Create<bool>() : testsMarked >= fails.Value;
                var user = testData.CreateUser();
                var userTag = testData.AddUserTag(user, testData.CriticalThinkingTag, testConfiguration.PointsRequiredToBeCompetent);
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
            target.MarkTest(markTest_UserPoint.Id, fixture.Create<bool>(), testData.Admin.UserId);

            //Assert
            Assert.IsFalse(markTest_UserPoint.MarkingComplete);
        }

        [TestCase(3)]
        [TestCase(4)]
        public void MarkTest_with_all_markers_sets_MarkingComplete_to_true(int markersRequiredPerTest)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1);
            var actualMarkingComplete = markTest_UserPoint.MarkingComplete;
            usersContext.SavedChanges += context =>
            {
                actualMarkingComplete = markTest_UserPoint.MarkingComplete;
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, fixture.Create<bool>(), testData.Admin.UserId);

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
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(markersTagExpectedPoints, actualPoints);
        }

        [TestCase(4, 5)]
        public void MarkTest_with_all_markers_and_50_50_split_reduces_all_markers_points(int markersRequiredPerTest, int pointsToLose)
        {
            //Arrange
            testConfiguration.PointsMarkersLoseForDisagreeingATestResult = pointsToLose;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, markersRequiredPerTest / 2);
            var markersTagExpectedPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints - pointsToLose).ToList();

            List<int> actualPoints = null;
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_Markers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(markersTagExpectedPoints, actualPoints);
        }

        [TestCase(5, 5, 3)]
        public void MarkTest_with_all_markers_and_non_50_50_split_reduces_minority_markers_points(int markersRequiredPerTest, int pointsToLose, int numberOfFails)
        {
            //Arrange
            testConfiguration.PointsMarkersLoseForDisagreeingATestResult = pointsToLose;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);
            var minorityExpectedPoints = markTest_PassMarkers_UserTags.Select(tag => tag.TotalPoints - pointsToLose).ToList();

            List<int> actualPoints = null;
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_PassMarkers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(minorityExpectedPoints, actualPoints);
        }

        [TestCase(5, 7, 3)]
        public void MarkTest_with_all_markers_and_non_50_50_split_increases_majority_markers_points(int markersRequiredPerTest, int pointsToGain, int numberOfFails)
        {
            //Arrange
            testConfiguration.PointsMarkersGainForAgreeingATestResult = pointsToGain;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);
            var majorityExpectedPoints = markTest_FailMarkers_UserTags.Select(tag => tag.TotalPoints + pointsToGain).ToList();

            List<int> actualPoints = null;
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_FailMarkers_UserTags.Select(tag => tag.TotalPoints).ToList();
            };

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            CollectionAssert.AreEqual(majorityExpectedPoints, actualPoints);
        }

        [TestCase(5, 7, 2)]
        public void MarkTest_with_all_markers_and_majority_of_markers_passed_increases_markers_points(int markersRequiredPerTest, int pointsToGain, int numberOfFails)
        {
            //Arrange
            testConfiguration.PointsForPassingATest = pointsToGain;
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var actualPoints = markTest_UserPoint.Quantity;
            var totalPoints = testData.JoesCriticalThinkingTag.TotalPoints;
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_UserPoint.Quantity;
                totalPoints = testData.JoesCriticalThinkingTag.TotalPoints;
            };
            var expectedPoints = pointsToGain;
            // e.g. if total = 10, actualPoints = -2, and pointsToGain = 7, then expected total = 19
            var expectedTotalPoints = totalPoints + pointsToGain - actualPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            Assert.AreEqual(expectedPoints, actualPoints);
            Assert.AreEqual(expectedTotalPoints, totalPoints);
        }

        [TestCase(5, 3)]
        public void MarkTest_with_all_markers_and_majority_of_markers_failed_doesnt_change_markers_points(int markersRequiredPerTest, int numberOfFails)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var expectedPoints = markTest_UserPoint.Quantity;
            var expectedTotalPoints = testData.JoesCriticalThinkingTag.TotalPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

            //Assert
            Assert.AreEqual(expectedPoints, markTest_UserPoint.Quantity);
            Assert.AreEqual(expectedTotalPoints, testData.JoesCriticalThinkingTag.TotalPoints);
        }

        [TestCase(6, 3)]
        public void MarkTest_with_all_markers_and_50_50_split_resets_markers_points(int markersRequiredPerTest, int numberOfFails)
        {
            //Arrange
            ArrangeMarkTest(markersRequiredPerTest, markersRequiredPerTest - 1, numberOfFails);

            var actualPoints = markTest_UserPoint.Quantity;
            var totalPoints = testData.JoesCriticalThinkingTag.TotalPoints;
            usersContext.SavedChanges += context =>
            {
                actualPoints = markTest_UserPoint.Quantity;
                totalPoints = testData.JoesCriticalThinkingTag.TotalPoints;
            };
            var expectedPoints = 0;
            // e.g. if total = 10, and actualPoints = -2, then expected total = 12
            var expectedTotalPoints = totalPoints - actualPoints;

            //Act
            target.MarkTest(markTest_UserPoint.Id, true, testData.Admin.UserId);

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
            testData.StephensCriticalThinkingTag.TotalPoints = 
                (knowledgeLevel == KnowledgeLevel.Competent 
                ? testConfiguration.PointsRequiredToBeCompetent
                : testConfiguration.PointsRequiredToBeAnExpert)
                + (int)userPointsForTag;

            getTicketTagInfoList_Ticket = new Suggestion
            {
                User = testData.Joe,
                FkUserId = testData.Joe.UserId,
                TicketTags = new List<TicketTag>
                {
                    new TicketTag
                    {
                        Verified = false,
                        CreatedBy = testData.Joe,
                        FkCreatedById = testData.Joe.UserId,
                        Tag = testData.CriticalThinkingTag,
                        FkTagId = testData.CriticalThinkingTag.Id
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
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

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
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

            //Assert
            Assert.IsTrue(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_verified_returns_UserCanValidate_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().Verified = true;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_created_by_user_returns_UserCanValidate_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList();
            getTicketTagInfoList_Ticket.TicketTags.Single().CreatedBy = testData.Stephen;
            getTicketTagInfoList_Ticket.TicketTags.Single().FkCreatedById = testData.Stephen.UserId;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

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
            getTicketTagInfoList_Ticket.User = testData.Stephen;
            getTicketTagInfoList_Ticket.FkUserId = testData.Stephen.UserId;

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_user_has_less_than_competent_points_for_that_tag_returns_UserCanValidate_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList(Relative.LessThan);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

            //Assert
            Assert.IsFalse(actual.UserMayVerify);
        }

        [Test]
        public void GetTicketTagInfoList_user_has_less_than_expert_points_for_that_tag_returns_UserIsExpert_is_false()
        {
            //Arrange
            ArrangeGetTicketInfoTagList(Relative.LessThan, KnowledgeLevel.Expert);

            //Act
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

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
            var actual = target.GetTicketTagInfoList(getTicketTagInfoList_Ticket, testData.Stephen.UserId).Single();

            //Assert
            Assert.IsTrue(actual.UserIsExpert);
        }

        #endregion GetTicketTagInfoList

        #region StartTest
        
        [Test]
        public void StartTest_with_user_without_associated_tag_adds_tag_to_user()
        {
            //Arrange
            UserTag stephensScubaDivingTag = null;
            usersContext.SavedChanges += context =>
            {
                stephensScubaDivingTag = context.UserTags.SingleOrDefault(tag => 
                    tag.FkTagId == testData.ScubaDivingTag.Id &&
                    tag.FkUserId == testData.Stephen.UserId);
            };

            //Act
            target.StartTest(testData.ScubaDivingTestThatJoeTook, testData.Stephen.UserId);

            //Assert
            Assert.IsNotNull(stephensScubaDivingTag);
        }

        [TestCase(1)]
        [TestCase(0)]
        public void StartTest_with_points_higher_than_or_equal_to_required_points_deducts_points(int pointsAboveRequired)
        {
            //Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = testConfiguration.PointsRequiredBeforeDeductingPoints + pointsAboveRequired;
            int? actual = null;
            usersContext.SavedChanges += context =>
            {
                var newPoint = context.UserPoints.Single(point =>
                    point.FkTagId == testData.CriticalThinkingTag.Id &&
                    point.FkTestId == testData.CriticalThinkingTestThatJoeTook.Id &&
                    point.User == testData.Stephen);
                actual = newPoint.Quantity;
            };

            //Act
            target.StartTest(testData.CriticalThinkingTestThatJoeTook, testData.Stephen.UserId);

            //Assert
            Assert.AreEqual(-testConfiguration.PointsToDeductWhenStartingTest, actual.Value);
        }

        [TestCase(1)]
        [TestCase(0)]
        public void StartTest_with_points_higher_than_or_equal_to_required_points_deducts_points_from_user_tag_total(int pointsAboveRequired)
        {
            //Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = testConfiguration.PointsRequiredBeforeDeductingPoints + pointsAboveRequired;
            var prevPoints = testData.StephensCriticalThinkingTag.TotalPoints;
            var actualTotalPoints = prevPoints;
            usersContext.SavedChanges += context =>
            {
                actualTotalPoints = testData.StephensCriticalThinkingTag.TotalPoints;
            };

            //Act
            target.StartTest(testData.CriticalThinkingTestThatJoeTook, testData.Stephen.UserId);

            //Assert
            Assert.AreEqual(prevPoints - testConfiguration.PointsToDeductWhenStartingTest, actualTotalPoints);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void StartTest_with_points_less_than_or_equal_to_required_points_does_not_deduct_points(int pointsBelowRequired)
        {
            //Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = testConfiguration.PointsRequiredBeforeDeductingPoints - pointsBelowRequired;

            //Act
            target.StartTest(testData.CriticalThinkingTestThatJoeTook, testData.Stephen.UserId);

            //Assert
            var newPoint = testData.UsersContext.UserPoints.Single(point =>
                point.FkTagId == testData.CriticalThinkingTag.Id &&
                point.FkTestId == testData.CriticalThinkingTestThatJoeTook.Id &&
                point.User == testData.Stephen);
            Assert.AreEqual(0, newPoint.Quantity);
        }

        #endregion StartTest

        #region Create
       
        [Test]
        public void Create_with_valid_parameters_sets_ModifiedDate()
        {
            //Arrange
            var ticket = new Question
            {
            };
            Ticket actualTicket = null;
            usersContext.SavedChanges += context =>
            {
                actualTicket = context.Tickets.Last();
            };            

            //Act
            target.Create(ticket, testData.Stephen.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        #endregion Create

        #region AddTicketTag

        [Test]
        public void AddTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = testData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            usersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.AddTicketTag(ticket.Id, testData.CriticalThinkingTag.Id, testData.Stephen.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        #endregion AddTicketTag

        #region DeleteTicketTag

        [Test]
        public void DeleteTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = testData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            usersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.DeleteTicketTag(ticket.Id, testData.ScubaDivingTag.Id, testData.Admin.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        #endregion DeleteTicketTag

        #region VerifyTicketTag

        [Test]
        public void VerifyTicketTag_with_valid_parameters_sets_ModifiedDate_on_ticket()
        {
            //Arrange
            var ticket = testData.JoesScubaDivingSuggestion;
            Ticket actualTicket = null;
            usersContext.SavedChanges += context =>
            {
                actualTicket = ticket;
            };

            //Act
            target.VerifyTicketTag(ticket.Id, testData.ScubaDivingTag.Id, testData.Admin.UserId);

            //Assert
            AssertModifiedDateSet(actualTicket.ModifiedDate);
        }

        #endregion VerifyTicketTag

        #region SetArgument

        private Argument setArgument_ExistingArgument;
        private Ticket setArgument_Ticket;

        private void ArrangeSetArgument(bool existingArgument = false)
        {
            setArgument_Ticket = testData.JoesScubaDivingSuggestion;
            setArgument_Ticket.State = TicketState.Discussion;

            setArgument_ExistingArgument = new Argument
            {
                FkTicketId = testData.JoesScubaDivingSuggestion.Id,
                FkUserId = testData.Admin.UserId
            };
            testData.UsersContext.Arguments.Add(setArgument_ExistingArgument);
         }

        [Test]
        public void SetArgument_with_valid_user_and_no_argument_adds_argument()
        {
            //Arrange
            ArrangeSetArgument();
            Argument newArgument = null;
            usersContext.SavedChanges += context =>
            {
                newArgument = context.Arguments.Last();
            };           

            //Act
            target.SetArgument(setArgument_Ticket.Id, testData.Admin.UserId, "hello");

            //Assert
            Assert.AreEqual("hello", newArgument.Body);
            Assert.AreEqual(testData.Admin.UserId, testData.Admin.UserId);
            AssertModifiedDateSet(testData.JoesScubaDivingSuggestion.ModifiedDate);
        }
      
        [Test]
        public void SetArgument_with_valid_user_and_existing_argument_updates_argument()
        {
            //Arrange
            ArrangeSetArgument(true);
            string actualArgument = null;
            usersContext.SavedChanges += context =>
            {
                actualArgument = setArgument_ExistingArgument.Body;
            };

            //Act
            target.SetArgument(setArgument_Ticket.Id, testData.Admin.UserId, "hello");

            //Assert
            Assert.AreEqual("hello", actualArgument);
            AssertModifiedDateSet(testData.JoesScubaDivingSuggestion.ModifiedDate);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetArgument_with_user_who_is_not_an_expert_for_the_ticket_throws_NotSupportedException()
        {
            //Arrange
            ArrangeSetArgument();
            testData.AdminsScubaDivingTag.TotalPoints = testConfiguration.PointsRequiredToBeAnExpert - 1;

            //Act
            target.SetArgument(setArgument_Ticket.Id, testData.Admin.UserId, "hello");
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
            target.SetArgument(setArgument_Ticket.Id, testData.Admin.UserId, "hello");
        }

        #endregion SetArgument

        #region ProgressTicketsWithVerifiedTags

        Ticket progressTicketsWithVerifiedTagsTicket;

        private void ArrangeProgressTicketsWithVerifiedTags()
        {
            progressTicketsWithVerifiedTagsTicket = testData.JoesCriticalThinkingSuggestion;
            progressTicketsWithVerifiedTagsTicket.State = TicketState.Verification;
            progressTicketsWithVerifiedTagsTicket.TicketTags.ForEach(tag => tag.Verified = true);
            testConfiguration.DelayAfterValidatingTags = TimeSpan.FromSeconds(10);
            progressTicketsWithVerifiedTagsTicket.ModifiedDate = DateTime.UtcNow.AddSeconds(-11);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_verified_ticket_changes_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            TicketState? actualState = null;
            DateTime? actualModifiedDate = null;
            Action updateTicket = () =>
            {
                actualState = progressTicketsWithVerifiedTagsTicket.State;
                actualModifiedDate = progressTicketsWithVerifiedTagsTicket.ModifiedDate;
            };
            updateTicket();
            usersContext.SavedChanges += context => updateTicket();

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreNotEqual(TicketState.Verification, actualState.Value);
            AssertModifiedDateSet(actualModifiedDate.Value);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_unverified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.State = TicketState.Draft;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Draft, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.ModifiedDate = DateTime.UtcNow.AddSeconds(-8);

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_without_tags_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            var tags = progressTicketsWithVerifiedTagsTicket.TicketTags.ToList();
            progressTicketsWithVerifiedTagsTicket.TicketTags.Clear();
            foreach(var tag in tags)
            {
                testData.UsersContext.TicketTags.Remove(tag);
            }

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_with_unverified_tag_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.TicketTags.Last().Verified = false;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        #endregion ProgressTicketsWithVerifiedTags

        private void AssertModifiedDateSet(DateTime modifiedDate)
        {
            Assert.GreaterOrEqual(modifiedDate, DateTime.UtcNow.AddSeconds(-3));
            Assert.LessOrEqual(modifiedDate, DateTime.UtcNow);
        }
    }
}