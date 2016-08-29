using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Services;
using Transparent.Data.Tests.Helpers;
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
        private Mock<IUsersContextFactory> mockUsersContextFactory;

        private Random random;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            random = new Random();

            mockDataService = new Mock<IDataService>();

            mockUsersContextFactory = new Mock<IUsersContextFactory>();
            mockUsersContextFactory.Setup(x => x.Create()).Returns(UsersContext);

            target = new ProgressTickets(mockUsersContextFactory.Object, mockDataService.Object, TestConfiguration, MockTags.Object);
        }

        #region ProgressTicketsInDiscussionState

        private Ticket ProgressTicketsInDiscussionState(int numberOfArguments = 1, TicketType ticketType = TicketType.Suggestion,
            int requiredNumberOfArguments = 2, int requiredNumberOfAnswers = 1)
        {
            Ticket ticket = null;
            switch(ticketType)
            {
                case TicketType.Suggestion:
                    ticket = TestData.JoesCriticalThinkingSuggestion;
                    break;
                case TicketType.Question:
                    ticket = TestData.JoesCriticalThinkingQuestion;
                    break;
                case TicketType.Test:
                    ticket = TestData.CriticalThinkingTestThatIsInTheVotingStage;
                    break;
                default:
                    throw new NotSupportedException("Unexpected ticket type: " + ticketType);
            }
            ticket.Rank = 500;
            ticket.TicketTags.ForEach(tag => tag.Verified = true);
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-11);
            TestConfiguration.MinimumNumberOfArgumentsToAdvanceState = requiredNumberOfArguments;
            TestConfiguration.MinimumNumberOfAnswersToAdvanceState = requiredNumberOfAnswers;
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

        private Ticket CreateTicket(TicketType ticketType = TicketType.Suggestion,
            int rank = 500, TicketState ticketState = TicketState.Discussion,
            int numberOfArguments = 0)
        {
            Ticket ticket = null;
            switch(ticketType)
            {
                case TicketType.Question:
                    ticket = new Question();
                    break;
                case TicketType.Suggestion:
                    ticket = new Suggestion();
                    break;
                case TicketType.Test:
                    ticket = new Test();
                    break;
            }
            ticket.Id = TestData.UsersContext.Tickets.Max(t => t.Id) + 1;
            ticket.FkUserId = TestData.Joe.UserId;
            ticket.User = TestData.Joe;
            ticket.Heading = ticketType.ToString() + ticket.Id;
            ticket.Body = ticket.Heading;
            ticket.Rank = rank;
            ticket.TicketTags.ForEach(tag => tag.TotalPoints = 5);
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-11);
            ticket.State = ticketState;
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-21);
            ticket.Arguments = new List<Argument>();
            for (int i = 0; i < numberOfArguments; i++)
            {
                ticket.Arguments.Add(new Argument
                {
                    FkTicketId = ticket.Id,
                    Ticket = ticket,
                    Body = Fixture.Create<string>()
                });
            }
            TestData.UsersContext.Tickets.Add(ticket);
            return ticket;
        }

        [TestCase(2, 2, 6, TicketType.Suggestion)]
        [TestCase(5, 4, 6, TicketType.Suggestion)]
        [TestCase(2, 6, 2, TicketType.Question)]
        [TestCase(5, 6, 4, TicketType.Question)]
        public void ProgressTicketsInDiscussionState_with_sufficient_arguments_changes_ticket_state(int numberOfArguments, 
            int requiredNumberOfArguments, int requiredNumberOfAnswers, TicketType ticketType)
        {
            //Arrange
            var ticket = ProgressTicketsInDiscussionState(numberOfArguments, ticketType, requiredNumberOfArguments, requiredNumberOfAnswers);
            bool setNextStateCalled = false;
            bool setNextStateCalledAndSaved = false;
            Action updateTicket = () =>
            {
                setNextStateCalledAndSaved = setNextStateCalled;
            };
            updateTicket();
            UsersContext.SavedChanges += context => updateTicket();
            mockDataService.Setup(x => x.SetNextState(ticket, null))
                .Callback(() => { setNextStateCalled = true; });

            //Act
            target.ProgressTicketsInDiscussionState();

            //Assert
            Assert.IsTrue(setNextStateCalledAndSaved);
        }

        [TestCase(3, 3, 0)]
        [TestCase(4, 5, 1)]
        [TestCase(5, 7, 2)]
        public void ProgressTicketsInDiscussionState_only_progresses_enough_suggestions_to_fill_voting_stage(
            int suggestionsInVotingState,
            int maximumNumberOfTicketsInVotingState, 
            int expectedNumberOfSuggestionsToMoveToVoting)
        {
            //Arrange
            TestConfiguration.MaximumNumberOfTicketsInVotingState = maximumNumberOfTicketsInVotingState;
            TestConfiguration.MinimumNumberOfArgumentsToAdvanceState = 0;
            TestConfiguration.MaxPositionToAdvanceState = 100;
            var topRankedDiscussionSuggestions = new List<Suggestion>();
            for(var i = 0; i < 10; i++)
            {
                topRankedDiscussionSuggestions.Add((Suggestion)CreateTicket(rank: 501 + i, ticketState:TicketState.Discussion));
            }
            foreach(var votingTicket in TestData.UsersContext.Tickets.Where(t => t.State == TicketState.Voting))
            {
                votingTicket.State = TicketState.Draft;
            }
            for (var i = 0; i < suggestionsInVotingState; i++)
            {
                CreateTicket(rank: 501 + i, ticketState: TicketState.Voting);
            }
            bool setNextStateCalledAndSaved = false;
            int actualNumberOfSuggestionsMoved = 0;

            foreach (var suggestion in topRankedDiscussionSuggestions)
            {
                bool setNextStateCalled = false;
                Action updateTicket = () =>
                {
                    setNextStateCalledAndSaved = setNextStateCalled;
                };
                updateTicket();
                UsersContext.SavedChanges += context => updateTicket();
                mockDataService.Setup(x => x.SetNextState(suggestion, null))
                    .Callback(() => 
                    {
                        setNextStateCalled = true;
                        actualNumberOfSuggestionsMoved++;
                    });
            }

            //Act
            target.ProgressTicketsInDiscussionState();

            //Assert
            if(expectedNumberOfSuggestionsToMoveToVoting > 0)
                Assert.IsTrue(setNextStateCalledAndSaved);
            Assert.AreEqual(expectedNumberOfSuggestionsToMoveToVoting, actualNumberOfSuggestionsMoved);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsInDiscussionState_with_enough_higher_ranked_tickets_in_the_same_ticket_state_doesnt_change_state(
            int numberOfHigherRankedTickets)
        {
            //Arrange
            var ticket = ProgressTicketsInDiscussionState();
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
            target.ProgressTicketsInDiscussionState();

            //Assert
            Assert.AreEqual(TicketState.Discussion, ticket.State);
        }

        [TestCase(TicketState.Draft)]
        public void ProgressTicketsInDiscussionState_with_non_discussion_ticket_does_not_change_ticket_state(TicketState ticketState)
        {
            //Arrange
            var ticket = ProgressTicketsInDiscussionState();
            ticket.State = ticketState;

            //Act
            target.ProgressTicketsInDiscussionState();

            //Assert
            Assert.AreEqual(ticketState, ticket.State);
        }

        [TestCase(2, 3, 0, TicketType.Suggestion)]
        [TestCase(5, 6, 0, TicketType.Suggestion)]
        [TestCase(2, 0, 3, TicketType.Question)]
        [TestCase(5, 0, 6, TicketType.Question)]
        public void ProgressTicketsInDiscussionState_with_insufficient_arguments_does_not_change_ticket_state(int numberOfArguments,
            int requiredNumberOfArguments, int requiredNumberOfAnswers, TicketType ticketType)
        {
            //Arrange
            var ticket = ProgressTicketsInDiscussionState(numberOfArguments, ticketType, requiredNumberOfArguments, requiredNumberOfAnswers);

            //Act
            target.ProgressTicketsInDiscussionState();

            //Assert
            mockDataService.Verify(x => x.SetNextState(ticket, It.IsAny<TicketState?>()), Times.Never);
        }

        [Test]
        public void ProgressTicketsInDiscussionState_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsInDiscussionState();
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-18);

            //Act
            target.ProgressTicketsInDiscussionState();

            //Assert
            Assert.AreEqual(TicketState.Discussion, ticket.State);
        }

        #endregion ProgressTicketsInDiscussionState

        #region ProgressTicketsWithVotes

        private Ticket ProgressTicketsWithVotes(int numberOfForVotes = 1, int numberOfAgainstVotes = 1, int secondsSinceModified = 16,
            int configuredSecondsForVoting = 15, int configuredPercentOfVotesRequiredToAccept = 60,
            TicketType ticketType = TicketType.Suggestion)
        {
            var ticket = ProgressTicketsInDiscussionState(ticketType: ticketType);
            ticket.State = TicketState.Voting;
            TestConfiguration.DelayForVoting = TimeSpan.FromSeconds(configuredSecondsForVoting);
            TestConfiguration.PercentOfVotesRequiredToAccept = configuredPercentOfVotesRequiredToAccept;
            ticket.ModifiedDate = DateTime.UtcNow.AddSeconds(-secondsSinceModified);
            ticket.VotesFor = numberOfForVotes;
            ticket.VotesAgainst = numberOfAgainstVotes;
            return ticket;
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithVotes_with_sufficient_time_passed_changes_ticket_state(int secondsSinceModified)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(secondsSinceModified: secondsSinceModified,
                configuredSecondsForVoting: secondsSinceModified - 1);
            bool setNextStateCalled = false;
            bool setNextStateCalledAndSaved = false;
            Action updateTicket = () =>
            {
                setNextStateCalledAndSaved = setNextStateCalled;
            };
            updateTicket();
            UsersContext.SavedChanges += context => updateTicket();
            mockDataService.Setup(x => x.SetNextState(ticket, It.IsAny<TicketState?>()))
                .Callback(() => { setNextStateCalled = true; });

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            Assert.IsTrue(setNextStateCalledAndSaved);
        }

        [TestCase(2)]
        [TestCase(5)]
        public void ProgressTicketsWithVotes_with_enough_higher_ranked_tickets_in_the_same_ticket_state_doesnt_change_state(
            int numberOfHigherRankedTickets)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes();
            for (var i = 0; i < numberOfHigherRankedTickets; i++)
            {
                var higherRankedTicket = new Suggestion
                {
                    Rank = ticket.Rank + i + 1,
                    State = TicketState.Voting
                };
                this.TestData.UsersContext.Tickets.Add(higherRankedTicket);
            }
            TestConfiguration.MaxPositionToAdvanceState = numberOfHigherRankedTickets;

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            Assert.AreEqual(TicketState.Voting, ticket.State);
        }

        [TestCase(3, 3, 0)]
        [TestCase(4, 5, 1)]
        [TestCase(5, 7, 2)]
        public void ProgressTicketsInVotingState_only_progresses_enough_suggestions_to_fill_accepted_stage(
            int suggestionsInAcceptedState,
            int maximumNumberOfTicketsInAcceptedState,
            int expectedNumberOfSuggestionsToMoveToAccepted)
        {
            //Arrange
            TestConfiguration.MaximumNumberOfTicketsInAcceptedState = maximumNumberOfTicketsInAcceptedState;
            TestConfiguration.MinimumNumberOfArgumentsToAdvanceState = 0;
            TestConfiguration.MaxPositionToAdvanceState = 100;
            var topRankedVotingSuggestions = new List<Suggestion>();
            for (var i = 0; i < 10; i++)
            {
                var ticket = (Suggestion)CreateTicket(rank: 501 + i, ticketState: TicketState.Voting);
                ticket.VotesFor = random.Next(1, 10);
                ticket.VotesAgainst = random.Next(1, 10);
                topRankedVotingSuggestions.Add
                (
                    ticket
                );
            }
            foreach (var acceptedTicket in TestData.UsersContext.Tickets.Where(t => t.State == TicketState.Accepted))
            {
                acceptedTicket.State = TicketState.Draft;
            }
            for (var i = 0; i < suggestionsInAcceptedState; i++)
            {
                CreateTicket(rank: 501 + i, ticketState: TicketState.Accepted);
            }
            bool setNextStateCalledAndSaved = false;
            int actualNumberOfSuggestionsMoved = 0;

            foreach (var suggestion in topRankedVotingSuggestions)
            {
                bool setNextStateCalled = false;
                Action updateTicket = () =>
                {
                    setNextStateCalledAndSaved = setNextStateCalled;
                };
                updateTicket();
                UsersContext.SavedChanges += context => updateTicket();
                mockDataService.Setup(x => x.SetNextState(suggestion, It.IsAny<TicketState?>()))
                    .Callback(() =>
                    {
                        setNextStateCalled = true;
                        actualNumberOfSuggestionsMoved++;
                    });
            }

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            if (expectedNumberOfSuggestionsToMoveToAccepted > 0)
                Assert.IsTrue(setNextStateCalledAndSaved);
            Assert.AreEqual(expectedNumberOfSuggestionsToMoveToAccepted, actualNumberOfSuggestionsMoved);
        }

        [Test]
        public void ProgressTicketsWithVotes_with_accepted_test_goes_to_completed_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(10, 0, ticketType: TicketType.Test);
 
            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.SetNextState(ticket, TicketState.Completed));
        }

        [Test]
        public void ProgressTicketsWithVotes_with_rejected_test_goes_to_rejected_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(0, 10, ticketType: TicketType.Test);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.SetNextState(ticket, TicketState.Rejected));
        }

        [TestCase(TicketState.Discussion)]
        public void ProgressTicketsWithVotes_with_non_voting_ticket_does_not_change_ticket_state(TicketState ticketState)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes();
            ticket.State = ticketState;

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            Assert.AreEqual(ticketState, ticket.State);
        }

        [TestCase(2, 2, 50)]
        [TestCase(61, 39, 60)]
        public void ProgressTicketsWithVotes_with_a_high_percentage_of_for_votes_accepts_ticket
            (int forVotes, int againstVotes, int requiredPercentage)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(forVotes, againstVotes, configuredPercentOfVotesRequiredToAccept: requiredPercentage);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.SetNextState(ticket, (TicketState?)TicketState.Accepted), Times.Once);
        }

        [TestCase(61, 39, 60)]
        public void ProgressTicketsWithVotes_with_a_high_percentage_of_for_votes_adds_DI_points
            (int forVotes, int againstVotes, int requiredPercentage)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(forVotes, againstVotes, configuredPercentOfVotesRequiredToAccept: requiredPercentage);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.AddPoints(It.IsAny<IUsersContext>(), ticket.FkUserId, 
                TestData.DemocraticIntelligenceTag.Id, TestConfiguration.DiPointsForAcceptedTicket, 
                PointReason.TicketAccepted, null, ticket.Id, null), Times.Once());
        }

        [TestCase(2, 2, 51)]
        [TestCase(59, 41, 60)]
        [TestCase(0, 0, 50)]
        public void ProgressTicketsWithVotes_with_a_low_percentage_of_for_votes_rejects_ticket
            (int forVotes, int againstVotes, int requiredPercentage)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(forVotes, againstVotes, configuredPercentOfVotesRequiredToAccept: requiredPercentage);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.SetNextState(ticket, (TicketState?)TicketState.Rejected), Times.Once);
        }

        [TestCase(0, 0, 50)]
        public void ProgressTicketsWithVotes_with_a_low_percentage_of_for_votes_does_not_add_DI_points
            (int forVotes, int againstVotes, int requiredPercentage)
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(forVotes, againstVotes, configuredPercentOfVotesRequiredToAccept: requiredPercentage);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            mockDataService.Verify(x => x.AddPoints(It.IsAny<IUsersContext>(), ticket.FkUserId,
                TestData.DemocraticIntelligenceTag.Id, TestConfiguration.DiPointsForAcceptedTicket,
                PointReason.TicketAccepted, It.IsAny<int?>(), null, null), Times.Never());
        }

        [Test]
        public void ProgressTicketsWithVotes_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            var ticket = ProgressTicketsWithVotes(secondsSinceModified:13);

            //Act
            target.ProgressTicketsWithVotes();

            //Assert
            Assert.AreEqual(TicketState.Voting, ticket.State);
        }

        #endregion ProgressTicketsWithVotes
    }
}
