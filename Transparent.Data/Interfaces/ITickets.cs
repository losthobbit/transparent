using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Data.ViewModels;

namespace Transparent.Data.Interfaces
{
    public interface ITickets
    {
        /// <summary>
        /// Returns list of tickets that have the same tag as the user.
        /// </summary>
        TicketsContainer MyQueue(TicketsContainer filter, int userId);

        TicketsContainer RaisedBy(TicketsContainer filter, int userId);
        TicketsContainer Newest(TicketsContainer filter);
        TicketsContainer HighestRanked(TicketsContainer filter);
        Search Search(Search filter);
        Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, int userId);
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

        IEnumerable<TicketDetailsViewModel.TagViewModel> GetTicketTagInfoList(Ticket ticket, int userId);
    }
}
