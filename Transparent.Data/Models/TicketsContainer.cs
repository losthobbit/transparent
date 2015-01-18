using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Data.Models
{
    public class TicketsContainer : StatefulPagedList<Ticket, TicketsContainer>
    {
        private const string TicketTypeKey = "T";

        public TicketsContainer()
        {
        }

        public TicketsContainer(IQueryable<Ticket> tickets): base(tickets)
        {
        }

        public TicketType? TicketType
        {
            get
            {
                return (TicketType?)GetNullableInt(TicketTypeKey);
            }
            set
            {
                SetValue(TicketTypeKey, (int?)value);
            }
        }

        /// <summary>
        /// Creates a generic TicketsContainer with a clone of the state and sets the TicketType of the cloned state.
        /// </summary>
        /// <param name="ticketType">The ticket type for the cloned state.</param>
        /// <returns>A generic TicketsContainer with a clone of the state.</returns>
        public Stateful GetState(TicketType? ticketType)
        {
            var stateful = GetState();
            stateful.SetValue(TicketTypeKey, (int?)ticketType);
            return stateful;
        }
    }
}