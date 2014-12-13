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
            ticketsContainer.PagedTickets.Single(ticket => ticket == testData.JoesCriticalThinkingTicket);
        }

        [TestMethod]
        public void MyQueue_with_ticket_and_user_with_same_tag_and_less_than_minimum_points_does_not_return_ticket()
        {
            // Arrange
            testData.StephensCriticalThinkingTag.TotalPoints = Tickets.MinimumUserTagPointsToWorkOnTicketWithSameTag - 1;

            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserName);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedTickets.Any(ticket => ticket == testData.JoesCriticalThinkingTicket));
        }

        [TestMethod]
        public void MyQueue_with_ticket_and_user_without_same_tag_does_not_return_ticket()
        {
            // Act
            var ticketsContainer = target.MyQueue(new TicketsContainer(), testData.Stephen.UserName);

            // Assert
            Assert.IsFalse(ticketsContainer.PagedTickets.Any(ticket => ticket == testData.JoesScubaDivingTicket));
        }

        #endregion MyQueue
    }
}
