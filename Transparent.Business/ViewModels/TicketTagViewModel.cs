using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Business.Maps;

namespace Transparent.Business.ViewModels
{
    public class TicketTagViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public bool UserMayVerify { get; set; }
        public bool UserMayDelete { get; set; }
        public bool UserIsExpert { get; set; }

        public static IEnumerable<TicketTagViewModel> CreateList(BaseTicket ticket, IEnumerable<TicketTagViewModel> source = null)
        {
            var tagInfoList = new List<TicketTagViewModel>(ticket.TicketTags.Select(ticketTag => new TicketTagViewModel { TagId = ticketTag.FkTagId, Name = ticketTag.Tag.Name }));

            if (source != null)
            {
                foreach (var ticketTagViewModel in tagInfoList)
                {
                    ticketTagViewModel.UserMayVerify = source.Any(tagInfo => tagInfo.TagId == ticketTagViewModel.TagId && tagInfo.UserMayVerify);
                }
            }

            return tagInfoList;
        }
    }

}
