using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Unique bit specifying a badge that a user can have.
    /// </summary>
    /// <remarks>
    /// All badges must be a unique power of 2, e.g. 1, 2, 4, 8.
    /// </remarks>
    public enum Badge
    {
        FirstTicketCreated = 1,
        FirstTestMarked = 2,
        FirstArgument = 4,
        FirstTestAnswered = 8
    }
}
