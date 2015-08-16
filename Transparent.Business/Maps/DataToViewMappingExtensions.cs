using System;
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
                StateHint = source.StateHint,
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

        public static TicketDetailsViewModel Map(this Data.Models.Ticket source, int userId)
        {
            var viewModel = Map((Data.Models.BaseTicket)source);
            viewModel.Vote = source.Map<VoteViewModel>(userId);
            if (viewModel.State >= TicketState.Accepted && source.AssignedUser != null)
            {
                viewModel.AssignedName = source.AssignedUser.UserName;
            }
            return viewModel;
        }

        public static VoteViewModel Map<T>(this Data.Models.Ticket source, int userId)
            where T: VoteViewModel
        {
            return new VoteViewModel
            {
                TicketId = source.Id,
                UserVote = source.GetUserVote(userId),
                VotesFor = source.VotesFor,
                VotesAgainst = source.VotesAgainst,
                UserMayVote = source.State == TicketState.Voting && 
                    Dependencies.TicketsFactory.Create().UserHasCompetence(source.Id, userId)
            };
        }

        public static VolunteerViewModel Map<T>(this Data.Models.UserProfile source)
            where T : VolunteerViewModel
        {
            return new VolunteerViewModel
            {
                Services = source.Services,
                UserSummary = source.Map()
            };
        }

        public static IEnumerable<ArgumentViewModel> Map(this IEnumerable<Data.Models.Argument> source)
        {
            if (source == null)
                return null;
            return source
                .OrderByDescending(argument => argument.UserWeighting)
                .Select(Map);
        }

        public static ArgumentViewModel Map(this Data.Models.Argument source)
        {
            return new ArgumentViewModel
            {
                Body = source.Body,
                FkUserId = source.FkUserId,
                User = source.User.Map(),
                UserWeighting = source.UserWeighting
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

        public static UserTagViewModel Map(this Data.Models.Tag tag)
        {
            return new UserTagViewModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                CompetentPoints = tag.CompetentPoints,
                ExpertPoints = tag.ExpertPoints
            };
        }

        public static UserTagViewModel Map(this Data.Models.Tag tag, int userId)
        {
            var userTagViewModel = tag.Map();
            var userTag = Dependencies.UserFactory.Create().GetUserTag(userId, tag.Id);
            return userTagViewModel.Map(userTag);
        }

        public static UserTagViewModel Map(this Data.Models.UserTag source)
        {
            return source.Tag.Map().Map(source);
        }

        public static UserTagViewModel Map(this UserTagViewModel destination, Data.Models.UserTag source)
        {
            destination.TotalPoints = source == null ? 0 : source.TotalPoints;
            destination.KnowledgeLevel = Dependencies.Tags.GetKnowledgeLevel(source);
            return destination;
        }

        public static TicketDetailsViewModel Map(this TicketDetailsViewModel destination, Data.Models.Stance source)
        {
            destination.UserRank = source;
            return destination;
        }
    }
}
