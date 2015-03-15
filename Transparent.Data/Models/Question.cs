﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Question : Ticket
    {
        private static TicketState[] states = new[]
        {
            TicketState.Verification,
            TicketState.Argument,
            TicketState.Voting
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

        protected override IEnumerable<TicketState> States
        {
            get { return states; }
        }
    }
}
