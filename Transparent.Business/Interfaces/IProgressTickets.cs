using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Business.Interfaces
{
    public interface IProgressTickets
    {
        /// <summary>
        /// Progresses tickets which are in the Discussion state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        void ProgressTicketsInDiscussionState();

        /// <summary>
        /// Progresses tickets which are in the Voting state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        void ProgressTicketsWithVotes();
    }
}
