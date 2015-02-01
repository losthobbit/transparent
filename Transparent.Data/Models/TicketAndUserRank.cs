using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TicketAndUserRank : BaseTicket
    {
        private Ticket ticket { get; set; }

        public TicketRank UserRank { get; set; }

        public TicketAndUserRank()
        {

        }

        public TicketAndUserRank(Ticket ticket, TicketRank userRank)
        {
            this.ticket = ticket;
            Id = ticket.Id;
            Rank = ticket.Rank;
            UserRank = userRank;
            Heading = ticket.Heading;
            Body = ticket.Body;
            CreatedDate = ticket.CreatedDate;
            TicketType = ticket.TicketType;
            TicketTags = ticket.TicketTags;
        }

        public override string TextForCreated
        {
            get
            {
                return ticket.TextForCreated;
            }
        }
    }
}
