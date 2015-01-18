using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Used in views for viewing paged lists and keeping track of their page number and how they have been filtered.
    /// </summary>
    /// <typeparam name="TItem">The type of list item.</typeparam>
    /// <typeparam name="TContainer">The type of list.</typeparam>
    public class StatefulPagedList<TItem, TContainer> : Stateful
        where TContainer : StatefulPagedList<TItem, TContainer>
    {
        private const int pageSize = 10;
        private const string PageIndexKey = "P";

        public StatefulPagedList()
            : base()
        {
            PagedList = new PagedList<TItem>(pageSize);
        }

        public StatefulPagedList(IQueryable<TItem> items)
            : this()
        {
            Initialize(items);
        }

        public PagedList<TItem> PagedList { get; set; }

        public TContainer Initialize(IQueryable<TItem> items)
        {
            PagedList.Initialize(items, PageIndex);
            return (TContainer)this;
        }

        /// <summary>
        /// Uses parameters, such as TicketType to apply a filter.
        /// </summary>
        /// <param name="tickets">Query to add filter to.</param>
        /// <returns>Query with filter.</returns>
        public virtual IQueryable<TItem> ApplyFilter(IQueryable<TItem> items)
        {
            return items;
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
    }
}
