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
        TicketsContainer MyQueue(TicketsContainer filter, string userName);

        TicketsContainer RaisedBy(TicketsContainer filter, string userName);
        TicketsContainer Newest(TicketsContainer filter);
        TicketsContainer HighestRanked(TicketsContainer filter);
        Search Search(Search filter);
        Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, string userName);
        IEnumerable<Test> GetUntakenTests(int tagId, string userName);
        Test GetRandomUntakenTest(int tagId, string userName);
        int CountUntakenTestsRemaining(int tagId, string userName);

        /// <summary>
        /// Record that the user started the test and deduct points
        /// </summary>
        /// <param name="test">The test to start.</param>
        /// <exception cref="NotSupportedException">Test already completed.</exception>
        /// <exception cref="ArgumentNullException">Required argument is null.</exception>
        void StartTest(Test test, string userName);

        /// <exception cref="NotSupportedException">Test not started or already completed.</exception>
        void AnswerTest(int testId, string answer, string userName);

        AnsweredTests TestsToBeMarked(AnsweredTests filter, string markersUserName);

        TestAndAnswerViewModel TestToBeMarked(int userPointId, string markersUserName);
    }
}
