using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Data.Services;
using Transparent.Data.Tests.Helpers;

namespace Transparent.Data.Tests.Services
{
    [TestFixture]
    public class DataServiceTests : BaseTests
    {
        private DataService target;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            target = new DataService(MockTags.Object);
        }

        #region SetNextState

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

        [TestCase(TicketType.Suggestion, TicketState.Accepted, TicketState.Accepted)]
        [TestCase(TicketType.Suggestion, TicketState.Rejected, TicketState.Rejected)]
        [TestCase(TicketType.Test, TicketState.Accepted, TicketState.Completed)]
        [TestCase(TicketType.Test, TicketState.Rejected, TicketState.Rejected)]
        public void SetNextState_with_specifiedState_sets_state_according_to_ticket_type(
            TicketType ticketType, TicketState specifiedState, TicketState expectedState)
        {
            //Arrange
            Ticket ticket = null;
            switch (ticketType)
            {
                case TicketType.Suggestion: ticket = new Suggestion(); break;
                case TicketType.Test: ticket = new Test(); break;
            }
            ticket.State = TicketState.Voting;
            ticket.ModifiedDate = DateTime.UtcNow.AddDays(-1);

            //Act
            target.SetNextState(ticket, specifiedState);

            //Assert
            Assert.AreEqual(expectedState, ticket.State);
            AssertModifiedDateSet(ticket.ModifiedDate);
        }

        #endregion SetNextState

        #region SetVote

        private Ticket GetSetVoteTicket(int ticketVotesFor, int ticketVotesAgainst, Stance ticketUserVote)
        {
            var ticket = TestData.JoesScubaDivingSuggestion;
            ticket.VotesFor = ticketVotesFor;
            ticket.VotesAgainst = ticketVotesAgainst;
            ticket.UserVotes = new List<TicketUserVote>
            {
            };
            if (ticketUserVote != Stance.Neutral)
            {
                ticket.UserVotes.Add
                (
                    new TicketUserVote
                    {
                        FkTicketId = ticket.Id,
                        For = ticketUserVote == Stance.For,
                        FkUserId = TestData.Stephen.UserId,
                        Ticket = ticket,
                        User = TestData.Stephen
                    }
                );
            }
            return ticket;
        }

        [TestCase(5, 7, Stance.Neutral, Stance.For, 6, 7)]
        [TestCase(5, 7, Stance.Neutral, Stance.Against, 5, 8)]
        [TestCase(5, 7, Stance.Neutral, Stance.Neutral, 5, 7)]
        [TestCase(5, 7, Stance.For, Stance.For, 5, 7)]
        [TestCase(5, 7, Stance.For, Stance.Against, 4, 8)]
        [TestCase(5, 7, Stance.For, Stance.Neutral, 4, 7)]
        [TestCase(5, 7, Stance.Against, Stance.For, 6, 6)]
        [TestCase(5, 7, Stance.Against, Stance.Against, 5, 7)]
        [TestCase(5, 7, Stance.Against, Stance.Neutral, 5, 6)]
        public void SetVote_with_valid_parameters_sets_VotesFor_and_VotesAgainst(
            int ticketVotesFor, int ticketVotesAgainst, Stance ticketUserVote,
            Stance newVote, int expectedVotesFor, int expectedVotesAgainst)
        {
            //Arrange
            var ticket = GetSetVoteTicket(ticketVotesFor, ticketVotesAgainst, ticketUserVote);

            //Act
            target.SetVote(ticket, newVote, TestData.Stephen.UserId);

            //Assert
            Assert.AreEqual(expectedVotesFor, ticket.VotesFor);
            Assert.AreEqual(expectedVotesAgainst, ticket.VotesAgainst);
        }

        public enum Crud { Create, Update, Delete, Nothing };

        [TestCase(5, 7, Stance.Neutral, Stance.For, Crud.Create)]
        [TestCase(5, 7, Stance.Neutral, Stance.Against, Crud.Create)]
        [TestCase(5, 7, Stance.Neutral, Stance.Neutral, Crud.Nothing)]
        [TestCase(5, 7, Stance.For, Stance.For, Crud.Nothing)]
        [TestCase(5, 7, Stance.For, Stance.Against, Crud.Update)]
        [TestCase(5, 7, Stance.For, Stance.Neutral, Crud.Delete)]
        [TestCase(5, 7, Stance.Against, Stance.For, Crud.Update)]
        [TestCase(5, 7, Stance.Against, Stance.Against, Crud.Nothing)]
        [TestCase(5, 7, Stance.Against, Stance.Neutral, Crud.Delete)]
        public void SetVote_with_valid_parameters_sets_user_vote(
            int ticketVotesFor, int ticketVotesAgainst, Stance ticketUserVote, Stance newVote, Crud expectedAction)
        {
            //Arrange
            var ticket = GetSetVoteTicket(ticketVotesFor, ticketVotesAgainst, ticketUserVote);
            var existingVotes = ticket.UserVotes.Where(vote => vote.User == TestData.Stephen).Select(vote => vote.For).ToList();

            //Act
            target.SetVote(ticket, newVote, TestData.Stephen.UserId);

            //Assert
            var actualVotes = ticket.UserVotes.Where(vote => vote.FkUserId == TestData.Stephen.UserId).Select(vote => vote.For);
            switch (expectedAction)
            {
                case Crud.Create:
                {
                    Assert.AreEqual(existingVotes.Count() + 1, actualVotes.Count());
                    Assert.AreEqual(newVote == Stance.For, ticket.UserVotes.Last().For);
                    break;
                }
                case Crud.Update:
                {
                    Assert.AreEqual(existingVotes.Count(), actualVotes.Count());
                    Assert.AreEqual(newVote == Stance.For, ticket.UserVotes.Last().For);
                    break;
                }
                case Crud.Delete:
                {
                    Assert.AreEqual(existingVotes.Count() - 1, actualVotes.Count());
                    break;
                }
                case Crud.Nothing:
                {
                    CollectionAssert.AreEquivalent(existingVotes, actualVotes);
                    break;
                }
            }
        }

        #endregion SetVote
    }
}
