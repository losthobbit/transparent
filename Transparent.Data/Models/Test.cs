using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Test : Ticket
    {
        private static Dictionary<TicketState, Hint> stateHints = new Dictionary<TicketState, Hint>
        {
            {
                TicketState.Voting, new Hint
                (
                    "Competent level users can vote for or against creating this test.  After a time period has elapsed, this test will either " +
                    "move to the Rejected or Completed state, based on whether the ratio of for to against votes reached the target ratio.  " +
                    "If it goes into the Completed state, users will be able to take the test.",
                    "/Home/HowItWorks#voteTest"
                )
            }, 
            {
                TicketState.Rejected, new Hint
                (
                    "The ratio of for to against votes did not reach the target ratio.",
                    "/Home/HowItWorks#voteTest"
                )
            },             
            { 
                TicketState.Completed, new Hint
                (
                    showState: false
                )
            }
        };

        public override TicketType TicketType
        {
            get
            {
                return TicketType.Test;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        public override bool MultipleTags
        {
            get
            {
                return false;
            }
        }

        [NotMapped()]
        public int TagId
        {
            get
            {
                if (TicketTags == null)
                    return -1;
                // Will thrown an exception if there are no tags.  This is fine when the tags have not yet been validated.
                return TicketTags.Single().FkTagId;
            }
            set
            {
                if(TicketTags == null)
                    TicketTags = new List<TicketTag>();
                TicketTags.Add( new TicketTag { FkTagId = value } );
            }
        }

        protected override Dictionary<TicketState, Hint> StateHints
        {
            get { return stateHints; }
        }

        public override void TrySetState(TicketState state)
        {
            if (state == TicketState.Accepted)
                State = TicketState.Completed;
            else
                base.TrySetState(state);
        }
    }
}
