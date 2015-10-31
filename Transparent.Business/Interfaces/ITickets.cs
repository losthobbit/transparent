using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Business.ViewModels;

namespace Transparent.Business.Interfaces
{
    public interface ITickets
    {
        /// <summary>
        /// Returns list of tickets that have the same tag as the user.
        /// </summary>
        TicketsContainer MyQueue(TicketsContainer filter, int userId);

        TicketsContainer RaisedBy(TicketsContainer filter, int userId);
        /// <summary>
        /// Returns tickets in the filter's state.
        /// </summary>
        /// <returns>Tickets in the filter's state</returns>
        TicketsContainer Newest(TicketsContainer filter, bool publicOnly = false);
        TicketsContainer HighestRanked(TicketsContainer filter, bool publicOnly = false);

        /// <summary>
        /// Returns questions in the completed state.
        /// </summary>
        /// <returns>Questions in the completed state</returns>
        TicketsContainer Answered(TicketsContainer filter);

        Search Search(Search filter);

        /// <summary>
        /// Adjusts the rank of the ticket based on the user's stance.
        /// </summary>
        /// <returns>
        /// The new rank
        /// </returns>
        int SetRank(int ticketId, Stance ticketRank, int userId);

        IEnumerable<Test> GetUntakenTests(int tagId, int userId);
        Test GetRandomUntakenTest(int tagId, int userId);
        int CountUntakenTestsRemaining(int tagId, int userId);

        /// <summary>
        /// Record that the user started the test and deduct points
        /// </summary>
        /// <param name="test">The test to start.</param>
        /// <exception cref="NotSupportedException">Test already completed.</exception>
        /// <exception cref="ArgumentNullException">Required argument is null.</exception>
        void StartTest(Test test, int userId);

        /// <exception cref="NotSupportedException">Test not started or already completed.</exception>
        void AnswerTest(int testId, string answer, int userId);

        AnsweredTests GetTestsToBeMarked(AnsweredTests filter, int markersUserId);

        TestAndAnswerViewModel GetTestToBeMarked(int userPointId, int markersUserId);

        void MarkTest(int userPointId, bool passed, int markersUserId);

        Ticket FindTicket(int id);

        IEnumerable<TicketTagViewModel> GetTicketTagInfoList(Ticket ticket, int userId);
        IEnumerable<TicketTagViewModel> GetTicketTagInfoList(int ticketId, int userId);

        /// <exception cref="NotSupportedException">User may not delete tag.</exception>
        void DeleteTicketTag(int ticketId, int tagId, int userId);

        /// <exception cref="NotSupportedException">User may not verify tag.</exception>
        void VerifyTicketTag(int ticketId, int tagId, int userId);

        /// <exception cref="NotSupportedException">User may not vote for tag.</exception>
        /// <exception cref="InvalidOperationException">Ticket or ticket tag doesn't exist.</exception>
        void VoteForTicketTag(Stance stance, int ticketId, int tagId, int userId);

        /// <exception cref="NotSupportedException">User may not add tag.</exception>
        void AddTicketTag(int ticketId, int tagId, int userId);

        IQueryable<Tag> GetCompetentTags(int userId);

        void Create(Ticket ticket, int userId);

        void SetArgument(int ticketId, int userId, string argument);

        /// <summary>
        /// Adjusts the votes of the ticket based on the user's stance.
        /// </summary>
        VoteViewModel SetVote(int ticketId, Stance stance, int userId);

        /// <summary>
        /// Assigns a ticket to in progress, completed or accepted.
        /// </summary>
        AssignViewModel Assign(AssignViewModel assign, int userId);

        bool UserHasCompetence(int ticketId, int userId);
    }
}
