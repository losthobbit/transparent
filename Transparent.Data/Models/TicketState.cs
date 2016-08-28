using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// From Discussion to Voting is considered "Public".
    /// Accepted or InProgress is considered "Voluntary".
    /// </remarks>
    public enum TicketState
    {
        /// <summary>
        /// Ticket has been created, but not yet published.
        /// </summary>
        Draft = 5,

        /// <summary>
        /// Allows experts to write an argument about the ticket.  Users have the ability to verify a ticket's tags and add new tags.
        /// </summary>
        /// <remarks>
        /// Once a configured number of arguments have been written, the ticket goes to the next state.
        /// Not applicable to tests.
        /// </remarks>
        Discussion = 20,

        /// <remarks>
        /// Once a configured number of votes have been cast, and a configured amount of time has passed, the ticket goes to the next state.
        /// Can have more than one possible next state.
        /// Not applicable to questions
        /// </remarks>
        Voting = 30,

        /// <summary>
        /// The ticket has been either accepted or rejected.
        /// </summary>
        Rejected = 40,

        /// <summary>
        /// The ticket has been either accepted or rejected.
        /// </summary>
        Accepted = 50,

        /// <summary>
        /// The ticket is being worked on.
        /// </summary>
        InProgress = 60,

        /// <summary>
        /// All work has been done on the ticket
        /// </summary>
        Completed = 70
    }
}
