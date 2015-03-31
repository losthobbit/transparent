﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels;
using Common;
using Transparent.Data.Interfaces;

namespace Transparent.Business.Maps
{
    using Data.Models;

    public static class DataToViewMappingExtensions
    {
        /// <summary>
        /// Must be injected.
        /// </summary>
        /// <remarks>
        /// I know this is not good... but I don't care enough to care :)
        /// </remarks>
        public static Dependencies Dependencies { get; set; }

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
                Arguments = Map(source.Arguments).ToList()
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

        public static IEnumerable<ArgumentViewModel> Map(this IEnumerable<Data.Models.Argument> source)
        {
            if (source == null)
                return null;
            return source.Select(Map);
        }

        public static ArgumentViewModel Map(this Data.Models.Argument source)
        {
            return new ArgumentViewModel
            {
                Body = source.Body,
                FkUserId = source.FkUserId,
                User = source.User.Map()
            };
        }

        public static UserSummaryViewModel Map(this Data.Models.UserProfile source)
        {
            return new UserSummaryViewModel
            {
                Username = source.UserName,
                Tags = source.Tags.Map()
                    .Where(t => t.KnowledgeLevel > KnowledgeLevel.Beginner)
                    .OrderByDescending(t => t.TotalPoints)
                    .ToList()
            };
        }

        public static IEnumerable<UserTagViewModel> Map(this IEnumerable<Data.Models.UserTag> source)
        {
            if (source == null)
                return null;
            return source.Select(Map);
        }

        public static UserTagViewModel Map(this Data.Models.UserTag source)
        {
            return new UserTagViewModel
            {
                Id = source.Tag.Id,
                Name = source.Tag.Name,
                TotalPoints = source.TotalPoints,
                KnowledgeLevel = source.TotalPoints.ToKnowledgeLevel(Dependencies.Configuration.PointsRequiredToBeCompetent,
                    Dependencies.Configuration.PointsRequiredToBeAnExpert)
            };
        }

        public static TicketDetailsViewModel Map(this TicketDetailsViewModel destination, Data.Models.TicketRank source)
        {
            destination.UserRank = source;
            return destination;
        }
    }
}
