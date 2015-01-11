using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TicketAndUserRank : BaseTicket
    {
        public TicketRank UserRank { get; set; }

        public TicketAndUserRank()
        {

        }

        public TicketAndUserRank(Ticket ticket, TicketRank userRank)
        {
            Id = ticket.Id;
            Rank = ticket.Rank;
            UserRank = userRank;
            Heading = ticket.Heading;
            Body = ticket.Body;
            TicketType = ticket.TicketType;
            TicketTags = ticket.TicketTags;
        }
    }
}
