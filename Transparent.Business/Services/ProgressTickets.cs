using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.Services
{
    /// <summary>
    /// Contains ticket related events / scheduled tasks.
    /// </summary>
    /// <remarks>
    /// This is designed to be used as a singleton.
    /// </remarks>
    public class ProgressTickets : IProgressTickets
    {
        private readonly Func<IUsersContext> getUsersContext;
        private readonly IConfiguration configuration;
        private readonly IDataService dataService;

        public ProgressTickets(Func<IUsersContext> getUsersContext, IDataService dataService, IConfiguration configuration)
        {
            this.getUsersContext = getUsersContext;
            this.dataService = dataService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Progresses tickets which are in the Verification state, and were last modified
        /// the specified amount of time ago.
        /// </summary>
        public void ProgressTicketsWithVerifiedTags()
        {
            using (var db = getUsersContext())
            {
                var lastModified = DateTime.UtcNow - configuration.DelayAfterValidatingTags;
                var verifiedTickets = from ticket in db.Tickets
                                      where ticket.State == TicketState.Verification &&
                                      ticket.ModifiedDate <= lastModified &&
                                      ticket.TicketTags.Any() &&
                                      ticket.TicketTags.All(tag => tag.Verified)
                                      select ticket;

                foreach (var ticket in verifiedTickets)
                {
                    dataService.SetNextState(ticket);
                }
                db.SaveChanges();
            }
        }
    }
}
