using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Suggestion : Ticket
    {
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
    }
}
