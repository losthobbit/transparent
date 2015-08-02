using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Extensions
{
    public static class TicketExtensions
    {
        /// <summary>
        /// Tickets between verification and voting state.
        /// </summary>
        public static IQueryable<Ticket> GetPublic(this IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.State >= TicketState.Verification && t.State <= TicketState.Voting);
        }

        public static IQueryable<Ticket> ExcludeComplete(this IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.State != TicketState.Completed);
        }

        public static IQueryable<Ticket> ExcludeDraft(this IQueryable<Ticket> tickets)
        {
            return tickets.Where(t => t.State != TicketState.Draft);
        }
    }
}
