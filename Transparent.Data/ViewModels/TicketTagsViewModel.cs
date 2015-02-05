using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.ViewModels
{
    /// <summary>
    /// Used to display tags in partial view so that tags can be verified.
    /// </summary>
    public class TicketTagsViewModel
    {
        public bool EnableTagButton { get; set; }
        public int TicketId { get; set; }
        public IEnumerable<TicketTagViewModel> TagInfo { get; set; }

        public TicketTagsViewModel()
        {

        }

        public TicketTagsViewModel(BaseTicket ticket, bool enableTagButton)
        {
            EnableTagButton = enableTagButton;
            TicketId = ticket.Id;

            TagInfo = new List<TicketTagViewModel> (ticket.TicketTags.Select(ticketTag => new TicketTagViewModel { TagId = ticketTag.FkTagId, Name = ticketTag.Tag.Name }));

            var ticketDetailsViewModel = ticket as TicketDetailsViewModel;
            if (ticketDetailsViewModel != null && ticketDetailsViewModel.TagInfo != null)
            {
                foreach(var ticketTagViewModel in TagInfo)
                {
                    ticketTagViewModel.UserMayVerify = ticketDetailsViewModel.TagInfo.Any(tagInfo => tagInfo.TagId == ticketTagViewModel.TagId && tagInfo.UserMayVerify);
                }
            }
        }
    }
}
