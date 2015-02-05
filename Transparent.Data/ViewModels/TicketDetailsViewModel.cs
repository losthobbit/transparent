using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.ViewModels
{
    public class TicketDetailsViewModel : BaseTicket
    {
        private Ticket ticket { get; set; }

        public TicketRank UserRank { get; set; }

        public IEnumerable<TicketTagViewModel> TagInfo { get; set; }

        public TicketDetailsViewModel()
        {

        }

        public TicketDetailsViewModel(Ticket ticket, TicketRank userRank, IEnumerable<TicketTagViewModel> tagInfo)
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
            TagInfo = tagInfo;
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
