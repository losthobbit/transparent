using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels;

namespace Transparent.Business.Maps
{
    public static class ViewToDataMappingExtensions
    {
        /// <summary>
        /// Maps a ticket from the view to the data model.
        /// </summary>
        /// <remarks>
        /// This is probably not a good idea.  I did this because changing the way things work will probably
        /// require a big change.  It may be worth refactoring this at some point.
        /// </remarks>
        public static Data.Models.Ticket Map(this TicketDetailsViewModel source)
        {
            Data.Models.Ticket destination = null;
            switch (source.TicketType)
            {
                case Data.Models.TicketType.Question: destination = new Data.Models.Question(); break;
                case Data.Models.TicketType.Suggestion: destination = new Data.Models.Suggestion(); break;
                case Data.Models.TicketType.Test: destination = new Data.Models.Test(); break;
            }
            destination.Body = source.Body;
            destination.CreatedDate = source.CreatedDate;
            destination.Heading = source.Heading;
            destination.Id = source.Id;
            destination.Rank = source.Rank;
            destination.State = source.State;
            destination.TicketTags = source.TicketTags;
            return destination;
        }
    }
}
