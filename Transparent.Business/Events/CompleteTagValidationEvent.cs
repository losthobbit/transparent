﻿using Common.Interfaces;
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
    public class CompleteTagValidationEvent: TimedEvent
    {
        private readonly IProgressTickets progressTicketsService;

        public CompleteTagValidationEvent(Common.Interfaces.IConfiguration configuration, IProgressTickets progressTicketsService)
            : base(TimeSpan.Parse(configuration.GetValue("CompleteTagValidationEventInterval")))
        {
            this.progressTicketsService = progressTicketsService;
        }

        public override void Action()
        {
            progressTicketsService.ProgressTicketsWithVerifiedTags();
        }
    }
}
