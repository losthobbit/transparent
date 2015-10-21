using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Business.Maps;
using Transparent.Business.ViewModels.Interfaces;

namespace Transparent.Business.ViewModels
{
    public class TicketTagViewModel: BaseVoteViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public bool UserIsExpert { get; set; }

        public static IEnumerable<TicketTagViewModel> CreateList(BaseTicket ticket, IEnumerable<TicketTagViewModel> source = null)
        {
            var tagInfoList = new List<TicketTagViewModel>(ticket.TicketTags.Select(ticketTag => new TicketTagViewModel { TagId = ticketTag.FkTagId, Name = ticketTag.Tag.Name }));

            if (source != null)
            {
                foreach (var ticketTagViewModel in tagInfoList)
                {
                    ticketTagViewModel.UserMayVote = source.Any(tagInfo => tagInfo.TagId == ticketTagViewModel.TagId && tagInfo.UserMayVote);
                }
            }

            return tagInfoList;
        }

        // TODO: Check if this is correct
        public override int Id
        {
            get
            {
                return TagId;
            }
            set
            {
                TagId = value;
            }
        }
    }

}
