using Common.Interfaces;
using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;

namespace Transparent.Business.Events
{
    /// <summary>
    /// Used to move tickets to their next stage after tag validation is completed.
    /// </summary>
    public class CompleteTagValidationEvent: Event
    {
        private readonly Func<ITickets> getTicketsService;

        public CompleteTagValidationEvent(Common.Interfaces.IConfiguration configuration, Func<ITickets> getTicketsService)
            : base(TimeSpan.Parse(configuration.GetValue("CompleteTagValidationEventInterval")))
        {
            this.getTicketsService = getTicketsService;
        }

        public override void Action()
        {
            var tickets = getTicketsService();
            tickets.ProgressTicketsWithVerifiedTags();
        }
    }
}
