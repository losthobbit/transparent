﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Queries;
using Transparent.Data.Tests.Helpers;
using Common;
using NUnit.Framework;
using System.Security;

namespace Transparent.Data.Tests.Queries
{
    [TestFixture]
    public class TicketsTests
    {
        private Tickets target;

        private TestData testData;
        private IConfiguration testConfiguration;
        private IUsersContext usersContext;

        [SetUp()]
        public void SetUp()
        {
            testData = TestData.Create();
            usersContext = testData.UsersContext;
            testConfiguration = new TestConfig();
            target = new Tickets(usersContext, testConfiguration);
        }

        #region MyQueue

        [Test]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_more_than_or_equal_minimum_points_returns_ticket()
        {
            // Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag;

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

        #endregion MyQueue

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
    }
}
