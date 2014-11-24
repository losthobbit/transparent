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

        public Tickets(DbSet<Ticket> tickets)
        {
            this.tickets = tickets;
        }

        public TicketsContainer Newest()
        {
            return new TicketsContainer
            {
                Tickets = from ticket in tickets
                          orderby ticket.CreatedDate descending
                          select ticket
            };
        }

        public Search Search(string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString))
                return null;
            var results = from ticket in tickets
                          where
                               ticket.Heading.Contains(searchString) ||
                               ticket.Body.Contains(searchString)
                          orderby ticket.CreatedDate descending
                          select ticket;
            return new Search { SearchString = searchString, Tickets = results };
        }

        public int IncreaseRank(int ticketId)
        {
            var ticket = tickets.Single(t => t.Id == ticketId);
            return ++ticket.Rank;
        }

        public int DecreaseRank(int ticketId)
        {
            var ticket = tickets.Single(t => t.Id == ticketId);
            return --ticket.Rank;
        }
    }
}
