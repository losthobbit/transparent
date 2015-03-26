﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Services;
using Transparent.Business.Tests.Helpers;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Common;
using Ploeh.AutoFixture;

namespace Transparent.Business.Tests.Services
{
    [TestFixture]
    public class ProgressTicketsTests: BaseTests
    {
        ProgressTickets target;

        private Mock<IDataService> mockDataService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mockDataService = new Mock<IDataService>();

            target = new ProgressTickets(() => UsersContext, mockDataService.Object, TestConfiguration);
        }

        #region ProgressTicketsWithVerifiedTags

        private Ticket ProgressTicketsWithVerifiedTags()
        {
            var ticket = TestData.JoesCriticalThinkingSuggestion;
            ticket.State = TicketState.Verification;
            ticket.TicketTags.ForEach(tag => tag.Verified = true);
            TestConfiguration.DelayAfterValidatingTags = TimeSpan.FromSeconds(10);
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-11);
            return ticket;
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_verified_ticket_changes_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            bool setNextStateCalled = false;
            bool setNextStateCalledAndSaved = false;
            Action updateTicket = () =>
            {
                setNextStateCalledAndSaved = setNextStateCalled;
            };
            updateTicket();
            UsersContext.SavedChanges += context => updateTicket();
            mockDataService.Setup(x => x.SetNextState(ticket))
                .Callback(() => { setNextStateCalled = true; });

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.IsTrue(setNextStateCalledAndSaved);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithVerifiedTags_with_enough_higher_ranked_tickets_in_the_same_ticket_state_doesnt_change_state(
            int numberOfHigherRankedTickets)
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            for(var i = 0; i < numberOfHigherRankedTickets; i++)
            {
                var higherRankedTicket = new Suggestion
                {
                    Rank = ticket.Rank + i + 1,
                    TicketTags = new List<TicketTag>(),
                    State = TicketState.Verification
                };
                this.TestData.UsersContext.Tickets.Add(higherRankedTicket);
            }
            TestConfiguration.MaxPositionToAdvanceState = numberOfHigherRankedTickets;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, ticket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_unverified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            ticket.State = TicketState.Draft;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Draft, ticket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-8);

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, ticket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_without_tags_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            var tags = ticket.TicketTags.ToList();
            ticket.TicketTags.Clear();
            foreach (var tag in tags)
            {
                TestData.UsersContext.TicketTags.Remove(tag);
            }

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, ticket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_with_unverified_tag_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVerifiedTags();
            ticket.TicketTags.Last().Verified = false;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, ticket.State);
        }

        #endregion ProgressTicketsWithVerifiedTags

        #region ProgressTicketsWithArguments

        private Ticket ProgressTicketsWithArguments(int numberOfArguments = 1)
        {
            var ticket = ProgressTicketsWithVerifiedTags();
            ticket.State = TicketState.Discussion;
            TestConfiguration.DelayAfterDiscussion = TimeSpan.FromSeconds(20);
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-21);
            ticket.Arguments = new List<Argument>();
            for (int i = 0; i < numberOfArguments; i++)
            {
                ticket.Arguments.Add(new Argument{
                     FkTicketId = ticket.Id,
                     Ticket = ticket,
                     Body = Fixture.Create<string>()                    
                });
            }
            return ticket;
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithArguments_with_sufficient_arguments_changes_ticket_state(int numberOfArgumentsRequired)
        {
            //Arrange
            var ticket = ProgressTicketsWithArguments(numberOfArgumentsRequired);
            bool setNextStateCalled = false;
            bool setNextStateCalledAndSaved = false;
            Action updateTicket = () =>
            {
                setNextStateCalledAndSaved = setNextStateCalled;
            };
            updateTicket();
            UsersContext.SavedChanges += context => updateTicket();
            mockDataService.Setup(x => x.SetNextState(ticket))
                .Callback(() => { setNextStateCalled = true; });

            //Act
            target.ProgressTicketsWithArguments();

            //Assert
            Assert.IsTrue(setNextStateCalledAndSaved);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithArguments_with_enough_higher_ranked_tickets_in_the_same_ticket_state_doesnt_change_state(
            int numberOfHigherRankedTickets)
        {
            //Arrange
            var ticket = ProgressTicketsWithArguments();
            for (var i = 0; i < numberOfHigherRankedTickets; i++)
            {
                var higherRankedTicket = new Question
                {
                    Rank = ticket.Rank + i + 1,
                    Arguments = new List<Argument>(),
                    State = TicketState.Discussion
                };
                this.TestData.UsersContext.Tickets.Add(higherRankedTicket);
            }
            TestConfiguration.MaxPositionToAdvanceState = numberOfHigherRankedTickets;

            //Act
            target.ProgressTicketsWithArguments();

            //Assert
            Assert.AreEqual(TicketState.Discussion, ticket.State);
        }

        [TestCase(TicketState.Verification)]
        public void ProgressTicketsWithArguments_with_non_discussion_ticket_does_not_change_ticket_state(TicketState ticketState)
        {
            //Arrange
            var ticket = ProgressTicketsWithArguments();
            ticket.State = ticketState;

            //Act
            target.ProgressTicketsWithArguments();

            //Assert
            Assert.AreEqual(ticketState, ticket.State);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithArguments_with_insufficient_arguments_does_not_change_ticket_state(int numberOfArgumentsRequired)
        {
            //Arrange
            var ticket = ProgressTicketsWithArguments(numberOfArgumentsRequired - 1);

            //Act
            target.ProgressTicketsWithArguments();

            //Assert
            Assert.AreEqual(TicketState.Discussion, ticket.State);
        }

        [Test]
        public void ProgressTicketsWithArguments_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithArguments();
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-18);

            //Act
            target.ProgressTicketsWithArguments();

            //Assert
            Assert.AreEqual(TicketState.Discussion, ticket.State);
        }

        #endregion ProgressTicketsWithArguments
    }
}
