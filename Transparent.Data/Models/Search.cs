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

        public Search(string searchString, IQueryable<Ticket> tickets, int index): base(tickets, index)
        {
            this.SearchString = searchString;
        }

        [Display(Name = "Text to search for")]
        [MaxLength(200)]
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