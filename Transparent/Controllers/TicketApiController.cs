using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;
using Transparent.Models;
using WebMatrix.WebData;

namespace Transparent.Controllers
{
    /// <summary>
    /// Web services for ajax calls
    /// </summary>
    [Authorize]
    public class TicketApiController : Controller
    {
        private ITickets tickets;

        public TicketApiController(ITickets tickets)
        {
            this.tickets = tickets;
        }

        /// <summary>
        /// Removes a tag with Id of oldTagId, unless it is -1.  Adds a tag with the Id of newTagId.
        /// </summary>
        [HttpPost]
        public ActionResult UpdateTicketTag(UpdateTicketTagRequest request)
        {
            try
            {
                if (request.OldTagId > -1)
                    tickets.DeleteTicketTag(request.TicketId, request.OldTagId, WebSecurity.CurrentUserId);
                if (request.NewTagId > -1)
                    tickets.AddTicketTag(request.TicketId, request.NewTagId, WebSecurity.CurrentUserId);
                return Json(true);
            }
            // SO: I'm not sure whether to let the exception be raised or return false.
            catch (NotSupportedException)
            {
                return Json(false);
            }
        }
    }
}
