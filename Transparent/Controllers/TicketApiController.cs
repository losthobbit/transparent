using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Transparent.Data.Interfaces;

namespace Transparent.Controllers
{
    /// <summary>
    /// Web services for ajax calls
    /// </summary>
    public class TicketApiController : ApiController
    {
        private ITickets tickets;

        public TicketApiController(ITickets tickets)
        {
            this.tickets = tickets;
        }
    }
}
