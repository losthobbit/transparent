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
        private bool multipleTags { get; set; }

        public TicketRank UserRank { get; set; }

        public IEnumerable<TicketTagViewModel> TagInfo { get; set; }

        public TicketDetailsViewModel()
        {

        }

        public TicketDetailsViewModel(Ticket ticket, TicketRank userRank, IEnumerable<TicketTagViewModel> tagInfo)
        {
            this.ticket = ticket;
            multipleTags = ticket.MultipleTags;
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

        public override bool MultipleTags
        {
            get
            {
                return multipleTags;
            }
        }

        public TicketRank NewRank { get; set; }
    }
}
