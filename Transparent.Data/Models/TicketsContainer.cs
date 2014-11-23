using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Data.Models
{
    public class TicketsContainer
    {
        public TicketsContainer()
        {

        }

        public TicketsContainer(IQueryable<Ticket> tickets)
        {
            Tickets = tickets;
        }

        public IQueryable<Ticket> Tickets { get; set; }
    }
}