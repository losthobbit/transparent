using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transparent.Models
{
    public class UpdateTicketTagRequest
    {
        public int TicketId { get; set; }
        public int OldTagId { get; set; }
        public int NewTagId { get; set; }
    }
}