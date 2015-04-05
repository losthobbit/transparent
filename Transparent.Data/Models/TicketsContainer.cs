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
        private const string TicketStateKey = "Z";

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

        public TicketState? TicketState
        {
            get
            {
                return (TicketState?)GetNullableInt(TicketStateKey);
            }
            set
            {
                SetValue(TicketStateKey, (int?)value);
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

        /// <summary>
        /// Creates a generic TicketsContainer with a clone of the state and sets the TicketState of the cloned state.
        /// </summary>
        /// <param name="ticketState">The ticket state for the cloned state.</param>
        /// <returns>A generic TicketsContainer with a clone of the state.</returns>
        public Stateful GetState(TicketState? ticketState)
        {
            var stateful = GetState();
            stateful.SetValue(TicketStateKey, (int?)ticketState);
            return stateful;
        }
    }
}