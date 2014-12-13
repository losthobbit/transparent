using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data.Queries
{
    /// <summary>
    /// Contains methods for getting ticket queries.
    /// </summary>
    /// <remarks>
    /// Can be a singleton.
    /// </remarks>
    public class Tickets
    {
        /// <summary>
        /// One requires this number of total points on a tag in order to view it in My Queue.
        /// </summary>
        /// <remarks>
        /// Can be a configurable setting.
        /// </remarks>
        public const int MinimumUserTagPointsToWorkOnTicketWithSameTag = 1;

        private IDbSet<Ticket> tickets;
        private IDbSet<UserProfile> userProfiles;
        private IDbSet<TicketTag> ticketTags;
        private IDbSet<UserTag> userTags;

        public Tickets(IUsersContext usersContext)
        {
            this.tickets = usersContext.Tickets;
            this.userProfiles = usersContext.UserProfiles;
            this.ticketTags = usersContext.TicketTags;
            this.userTags = usersContext.UserTags;
        }

        /// <summary>
        /// Returns list of tickets that have the same tag as the user.
        /// </summary>
        public TicketsContainer MyQueue(TicketsContainer filter, string userName)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in tickets
                    join ticketTag in ticketTags on ticket equals ticketTag.Ticket
                    join userTag in userTags on ticketTag.Tag equals userTag.Tag
                    where userTag.User.UserName == userName && userTag.TotalPoints >= MinimumUserTagPointsToWorkOnTicketWithSameTag
                    select ticket
                ).OrderByDescending(ticket => ticket.Rank)                
            );
        }

        public TicketsContainer RaisedBy(TicketsContainer filter, string userName)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from ticket in tickets
                    where ticket.User.UserName == userName
                    select ticket
                ).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public TicketsContainer Newest(TicketsContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter(tickets).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public TicketsContainer HighestRanked(TicketsContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter(tickets).OrderByDescending(ticket => ticket.Rank)
            );
        }

        public Search Search(Search filter)
        {
            if (String.IsNullOrWhiteSpace(filter.SearchString))
                return null;
            return (Search)filter.Initialize
            (
                filter.ApplyFilter(tickets).OrderByDescending(ticket => ticket.CreatedDate)
            );
        }

        public Tuple<int, TicketRank> SetRank(int ticketId, TicketRank ticketRank, string userName)
        {
            var ticket = tickets.Single(t => t.Id == ticketId);
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.User.UserName == userName);
            if(rankRecord == null)
            {
                if(ticketRank != TicketRank.NotRanked)
                {
                    ticket.UserRanks.Add
                    (
                        new TicketUserRank
                        { 
                            Up = ticketRank == TicketRank.Up,
                            User = userProfiles.Single(user => user.UserName == userName)
                        }
                    );
                    ticket.Rank += (int)ticketRank;
                }
            }
            else
            {
                if(ticketRank == TicketRank.NotRanked)
                {
                    ticket.UserRanks.Remove(rankRecord);
                    ticket.Rank += rankRecord.Up ? -1 : 1;
                }
                else
                {
                    if(rankRecord.Up && ticketRank == TicketRank.Down || !rankRecord.Up && ticketRank == TicketRank.Up)
                    {
                        rankRecord.Up = !rankRecord.Up;
                        ticket.Rank += (int)ticketRank * 2;
                    }
                }
            }
            return new Tuple<int, TicketRank>(ticket.Rank, ticketRank);
        }
    }
}
