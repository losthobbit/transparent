using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Data.Models
{
    public class TicketsContainer : Stateful
    {
        private const int pageSize = 10;
        private const string PageIndexKey = "P";
        private const string TicketTypeKey = "T";

        public PagedList<Ticket> PagedTickets { get; set; }

        public TicketsContainer(): base()
        {
            PagedTickets = new PagedList<Ticket>(pageSize);
        }

        public TicketsContainer(IQueryable<Ticket> tickets): this()
        {
            Initialize(tickets);
        }

        public TicketsContainer Initialize(IQueryable<Ticket> tickets)
        {
            PagedTickets.Initialize(tickets, PageIndex);
            return this;
        }

        /// <summary>
        /// Uses parameters, such as TicketType to apply a filter.
        /// </summary>
        /// <param name="tickets">Query to add filter to.</param>
        /// <returns>Query with filter.</returns>
        public virtual IQueryable<Ticket> ApplyFilter(IQueryable<Ticket> tickets)
        {
            return tickets;
        }

        public int PageIndex
        {
            get
            {
                return GetInt(PageIndexKey);
            }
            set
            {
                SetValue(PageIndexKey, value);
            }
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
        /// Creates a generic TicketsContainer with a clone of the state and sets the PageIndex of the cloned state.
        /// </summary>
        /// <param name="pageIndex">The page index for the cloned state.</param>
        /// <returns>A generic TicketsContainer with a clone of the state.</returns>
        public Stateful GetState(int pageIndex)
        {
            var stateful = GetState();
            stateful.SetValue(PageIndexKey, pageIndex);
            return stateful;
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