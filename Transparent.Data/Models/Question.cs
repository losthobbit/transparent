﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Question : Ticket
    {
        private static Dictionary<TicketState, Hint> stateHints = new Dictionary<TicketState, Hint>
        {
            { 
                TicketState.Discussion, new Hint
                (
                    "Users can answer this question and verify the tags.  After a time period has passed without any further answers, this question " +
                    "will move to the Completed state.",
                    "/Home/HowItWorks#answer"
                )
            },
            { 
                TicketState.Completed, new Hint()
            }
        };

        public override TicketType TicketType
        {
            get
            {
                return TicketType.Question;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        public override string TextForCreated
        {
            get
            {
                return "Asked";
            }
        }

        public override string TextForArgument
        {
            get
            {
                return "answer";
            }
        }

        protected override Dictionary<TicketState, Hint> StateHints
        {
            get { return stateHints; }
        }
    }
}
