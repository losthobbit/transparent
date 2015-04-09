using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public class AssignViewModel
    {
        public int TicketId { get; set; }
        public TicketState TicketState { get; set; }
        public string Username { get; set; }
    }
}
