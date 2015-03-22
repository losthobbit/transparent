using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels;
using Common;

namespace Transparent.Business.Maps
{
    public static class DataToViewMappingExtensions
    {
        public static TicketDetailsViewModel Map(this Data.Models.BaseTicket source)
        {
            var viewModel = new TicketDetailsViewModel
            {
                MultipleTags = source.MultipleTags,
                Id = source.Id,
                Rank = source.Rank,
                Heading = source.Heading,
                Body = source.Body,
                CreatedDate = source.CreatedDate,
                TicketType = source.TicketType,
                TicketTags = source.TicketTags,
                State = source.State,
                TextForArgument = source.TextForArgument,
                Arguments = Map(source.Arguments)
            };
            viewModel.Arguments.ForEach(argument => 
            {
                argument.Caption = source.TextForArgument.CapitalizeFirstLetter();
                argument.FkTicketId = source.Id;
            });
            return viewModel;
        }

        public static TicketDetailsViewModel Map(this Data.Models.Ticket source)
        {
            return Map((Data.Models.BaseTicket)source);
        }

        public static IEnumerable<DiscussViewModel> Map(this IEnumerable<Data.Models.Argument> source)
        {
            if (source == null)
                return null;
            return source.Select(Map);
        }

        public static DiscussViewModel Map(this Data.Models.Argument source)
        {
            return new DiscussViewModel
            {
                Body = source.Body
            };
        }

        public static TicketDetailsViewModel Map(this TicketDetailsViewModel destination, Data.Models.TicketRank source)
        {
            destination.UserRank = source;
            return destination;
        }
    }
}
