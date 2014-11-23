using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Tickets
    {
        private DbSet<Ticket> tickets;

        public Tickets(DbSet<Ticket> tickets)
        {
            this.tickets = tickets;
        }

        public IQueryable<Ticket> RaisedBy(string userName)
        {
            return tickets.Where(ticket => ticket.User.UserName == userName);
        }

        public IQueryable<Ticket> All()
        {
            return tickets;
        }
    }
}
