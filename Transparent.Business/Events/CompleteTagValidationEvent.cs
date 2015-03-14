using Common.Interfaces;
using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Business.Events
{
    /// <summary>
    /// Used to move tickets to their next stage after tag validation is completed.
    /// </summary>
    public class CompleteTagValidationEvent: Event
    {
        public CompleteTagValidationEvent(IConfiguration configuration)
            : base(TimeSpan.Parse(configuration.GetValue("CompleteTagValidationEventInterval")))
        {
        }

        public override void Action()
        {
            Debug.WriteLine("Checking if tag validation is complete.");
        }
    }
}
