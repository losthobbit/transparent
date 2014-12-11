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
        public TicketsContainer MyQueue(int pageIndex, string userName)
        {
            return new TicketsContainer
            (
                from ticket in tickets
                join ticketTag in ticketTags on ticket equals ticketTag.Ticket
                join userTag in userTags on ticketTag.Tag equals userTag.Tag
                where userTag.User.UserName == userName && userTag.TotalPoints >= MinimumUserTagPointsToWorkOnTicketWithSameTag
                orderby ticket.Rank descending
                select ticket,
                pageIndex
            );
        }

        public TicketsContainer Newest(int pageIndex)
        {
            return new TicketsContainer
            (
                from ticket in tickets
                orderby ticket.CreatedDate descending
                select ticket,
                pageIndex
            );
        }

        public TicketsContainer HighestRanked(int pageIndex)
        {
            return new TicketsContainer
            (
                from ticket in tickets
                orderby ticket.Rank descending
                select ticket,
                pageIndex
            );
        }

        public Search Search(string searchString, int pageIndex)
        {
            if (String.IsNullOrWhiteSpace(searchString))
                return null;
            var results = from ticket in tickets
                          where
                               ticket.Heading.Contains(searchString) ||
                               ticket.Body.Contains(searchString)
                          orderby ticket.CreatedDate descending
                          select ticket;
            return new Search(searchString, results, pageIndex);
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
