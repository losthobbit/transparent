using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Business.Maps;
using Transparent.Business.ViewModels.Interfaces;
using Transparent.Data.Interfaces;

namespace Transparent.Business.ViewModels
{
    public class TicketTagViewModel: BaseVoteViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public bool UserIsExpert { get; set; }

        public static IEnumerable<TicketTagViewModel> CreateList(BaseTicket ticket,
            IThresholds thresholds, IEnumerable<TicketTagViewModel> source = null)
        {
            // Create a list with all the tags from the ticket
            var tagInfoList = new List<TicketTagViewModel>(ticket.TicketTags.Select(ticketTag => 
                new TicketTagViewModel 
                { 
                    TagId = ticketTag.FkTagId, 
                    Name = ticketTag.Tag.Name,
                    TotalPoints = ticketTag.TotalPoints,
                    NotAcceptedThreshold = thresholds.NotAcceptedThreshold,
                    FullAcceptanceThreshold = thresholds.FullAcceptanceThreshold
                }));

            // Assign details from the source
            if (source != null)
            {
                var tagPairs = from ticketTagViewModel in tagInfoList
                               join sourceTag in source 
                               on ticketTagViewModel.TagId equals sourceTag.TagId
                               select new { Destination = ticketTagViewModel, Source = sourceTag };

                foreach (var tagPair in tagPairs)
                {
                    tagPair.Destination.UserMayVote = tagPair.Source.UserMayVote;
                    tagPair.Destination.UserVote = tagPair.Source.UserVote;
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
