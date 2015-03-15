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
    /// From Verification to Voting is considered "Public".
    /// Accepted or InProgress is considered "Voluntary".
    /// </remarks>
    public enum TicketState
    {
        /// <summary>
        /// Ticket has been created, but not yet published.
        /// </summary>
        Draft = 5,

        /// <summary>
        /// After a ticket has been created, competents have the ability to verify a ticket's tags and add new tags.
        /// </summary>
        /// <remarks>
        /// After all tags have been verified and the ticket is the highest ranked, or the next highest ranked is in a higher state, 
        /// the ticket goes into the next state.
        /// </remarks>
        Verification = 10,

        /// <summary>
        /// Allows experts to write an argument about the ticket.
        /// </summary>
        /// <remarks>
        /// Once a configured number of arguments have been written, the ticket goes to the next state.
        /// Not applicable to tests.
        /// </remarks>
        Argument = 20,

        /// <remarks>
        /// Once a configured number of votes have been cast, and a configured amount of time has passed, the ticket goes to the next state.
        /// Can have more than one possible next state.
        /// Not applicable to tests
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
