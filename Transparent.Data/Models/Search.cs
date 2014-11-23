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
        public Search()
        {

        }

        public Search(IQueryable<Ticket> tickets): base(tickets)
        {

        }

        [Display(Name = "Text to search for")]
        [MaxLength(200)]
        public string SearchString { get; set; }
    }
}