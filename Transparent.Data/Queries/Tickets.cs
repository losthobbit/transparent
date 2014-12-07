using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Queries
{
    public class Tickets
    {
        private DbSet<Ticket> tickets;
        private DbSet<UserProfile> userProfiles;

        public Tickets(UsersContext dbContext)
        {
            this.tickets = dbContext.Tickets;
            this.userProfiles = dbContext.UserProfiles;
        }

        public TicketsContainer MyQueue(int pageIndex)
        {
            return new TicketsContainer
            (
                from ticket in tickets
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
