using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Data.Models
{
    public class Search : TicketsContainer
    {
        private const string SearchStringKey = "S";

        public Search():base()
        {

        }

        public Search(IQueryable<Ticket> tickets): base(tickets)
        {

        }

        public Search(string searchString, IQueryable<Ticket> tickets): this(tickets)
        {
            this.SearchString = searchString;
        }

        public override IQueryable<Ticket> ApplyFilter(IQueryable<Ticket> tickets)
        {
            return base.ApplyFilter
            (
                from ticket in tickets
                where ticket.Heading.Contains(SearchString) || ticket.Body.Contains(SearchString)
                select ticket
            );
        }

        [Display(Name = "Text to search for")]
        [StringLength(200)]
        public string SearchString 
        {
            get 
            {
                return GetValue(SearchStringKey);
            }
            set 
            {
                SetValue(SearchStringKey, value); 
            }
        }
    }
}