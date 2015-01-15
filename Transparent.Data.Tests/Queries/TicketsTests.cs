using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Queries;
using Transparent.Data.Tests.Helpers;

namespace Transparent.Data.Tests.Queries
{
    [TestClass]
    public class TicketsTests
    {
        private Tickets target;

        private TestData testData;
        private IUsersContext usersContext;

        [TestInitialize()]
        public void Initialize()
        {
            testData = TestData.Create();
            usersContext = testData.UsersContext;
            target = new Tickets(usersContext);
        }

        #region MyQueue

        [TestMethod]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_more_than_or_equal_minimum_points_returns_ticket()
        {
            // Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserName);

            // Assert
            ticketsContainer.PagedTickets.Single(ticket => ticket == testData.JoesCriticalThinkingSuggestion);
        }

        [TestMethod]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_less_than_minimum_points_does_not_return_ticket()
        {
            // Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag - 1;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserName);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedTickets.Any(ticket => ticket == testData.JoesCriticalThinkingSuggestion));
        }

        [TestMethod]
        public void MyQueue_with_ticket_and_user_without_same_tag_does_not_return_ticket()
        {
            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserName);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedTickets.Any(ticket => ticket == testData.JoesScubaDivingSuggestion));
        }

        #endregion MyQueue

        #region GetUntakenTests

        [TestMethod]
        public void GetUntakenTests_returns_only_tests_that_match_the_tag()
        {
            // Arrange
            var tag = testData.CriticalThinkingTag;
            var userName = testData.Stephen.UserName;

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userName);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => test.TicketTags.Single().Tag == tag));
        }

        [TestMethod]
        public void GetUntakenTests_returns_tests_that_have_not_been_taken_by_the_user()
        {
            // Arrange
            var tag = testData.CriticalThinkingTag;
            var userName = testData.Stephen.UserName;
            var stephensPoints = testData.UsersContext.UserPoints.Where(userPoints => userPoints.User == testData.Stephen);
            var testsStephenTook = stephensPoints.Select(point => point.TestTaken).Where(test => test != null).ToList();
            Assert.IsTrue(testsStephenTook.Any());

            // Act
            var actualTests = target.GetUntakenTests(tag.Id, userName);

            // Assert
            Assert.IsTrue(actualTests.Any());
            Assert.IsTrue(actualTests.All(test => !testsStephenTook.Contains(test)));
        }

        #endregion GetUntakenTest
    }
}
