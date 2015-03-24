using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Data.Services;

namespace Transparent.Data.Tests.Services
{
    [TestFixture]
    public class DataServiceTests
    {
        private DataService target;

        [SetUp]
        public void SetUp()
        {
            target = new DataService();
        }

        [TestCase(TicketState.InProgress)]
        [TestCase(TicketState.Verification)]
        public void SetNextState_with_ticket_with_next_state_changes_state(TicketState state)
        {
            //Arrange
            var ticket = new Suggestion{ State = state, ModifiedDate = DateTime.UtcNow.AddDays(-1) };

            //Act
            target.SetNextState(ticket);

            //Assert
            Assert.AreNotEqual(state, ticket.State);
            AssertModifiedDateSet(ticket.ModifiedDate);
        }

        private void AssertModifiedDateSet(DateTime modifiedDate)
        {
            Assert.GreaterOrEqual(modifiedDate, DateTime.UtcNow.AddSeconds(-3));
            Assert.LessOrEqual(modifiedDate, DateTime.UtcNow);
        }
    }
}
