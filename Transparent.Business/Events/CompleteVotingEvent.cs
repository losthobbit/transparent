using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;

namespace Transparent.Business.Events
{
    /// <summary>
    /// Used to move tickets to their next stage after voting is completed.
    /// </summary>
    public class CompleteVotingEvent: Event
    {
        private readonly IProgressTickets progressTicketsService;

        public CompleteVotingEvent(Common.Interfaces.IConfiguration configuration, IProgressTickets progressTicketsService)
            : base(TimeSpan.Parse(configuration.GetValue("CompleteVotingEventInterval")))
        {
            this.progressTicketsService = progressTicketsService;
        }

        public override void Action()
        {
            progressTicketsService.ProgressTicketsWithVotes();
        }
    }
}
