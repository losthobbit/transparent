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

        public PagedList<Ticket> PagedTickets { get; set; }

        public TicketsContainer(): base()
        {
            PagedTickets = new PagedList<Ticket>(pageSize);
        }

        public TicketsContainer(IQueryable<Ticket> tickets): this()
        {
            PagedTickets.Initialize(tickets, 0);
        }

        public TicketsContainer(IQueryable<Ticket> tickets, int index): this()
        {
            PagedTickets.Initialize(tickets, index);
        }

        public int PageIndex
        {
            get
            {
                return GetInt(PageIndexKey);
            }
            set
            {
                SetInt(PageIndexKey, value);
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
            stateful.SetInt(PageIndexKey, pageIndex);
            return stateful;
        }
    }
}