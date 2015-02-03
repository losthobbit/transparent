using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public enum TicketState
    {
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
        /// Not applicable to tests
        /// </remarks>
        Voting = 30,

        /// <summary>
        /// The ticket has been either accepted or rejected.
        /// </summary>
        Resolved = 40
    }
}
