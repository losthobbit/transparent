using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Suggestion : Ticket
    {
        private static TicketState[] states = new[]
        {
            TicketState.Verification,
            TicketState.Argument,
            TicketState.Voting,
            TicketState.Rejected,
            TicketState.Accepted,
            TicketState.InProgress,
            TicketState.Completed
        };

        public override TicketType TicketType
        {
            get
            {
                return TicketType.Suggestion;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        protected override IEnumerable<TicketState> States
        {
            get { return states; }
        }
    }
}
