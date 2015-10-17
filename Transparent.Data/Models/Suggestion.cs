using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Suggestion : Ticket
    {
        private static Dictionary<TicketState, Hint> stateHints = new Dictionary<TicketState, Hint>
        {
            { 
                TicketState.Discussion, new Hint
                (
                    "Users can discuss this suggestion and verify the tags.  After a time period has passed without any further arguments, this suggestion " +
                    "will move to the Voting state.",
                    "/Home/HowItWorks#argue"
                )
            },
            {
                TicketState.Voting, new Hint
                (
                    "Competent level users can vote for or against this suggestion.  After a time period has elapsed, this suggestion will either " +
                    "move to the Rejected or Accepted state, based on whether the ratio of for to against votes reached the target ratio.",
                    "/Home/HowItWorks#voteSuggestion"
                )
            }, 
            {
                TicketState.Rejected, new Hint
                (
                    "The ratio of for to against votes did not reach the target ratio.",
                    "/Home/HowItWorks#voteSuggestion"
                )
            },             
            {
                TicketState.Accepted, new Hint
                (
                    "The ratio of for to against votes reached the target ratio.  When it is picked up by a volunteer, it will move to the " +
                    "in progress state.",
                    "/Home/HowItWorks#voteSuggestion"
                )
            },             
            {
                TicketState.InProgress, new Hint
                (
                    "A volunteer has taken responsibility for implementing this suggestion.  When they have completed, they will move it to the " +
                    "Completed state."
                )
            }, 
            { 
                TicketState.Completed, new Hint
                (
                    "A volunteer has implemented this suggestion."
                )
            }
        };


        public override TicketType TicketType
        {
            get
            {
                return TicketType.Suggestion;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        protected override Dictionary<TicketState, Hint> StateHints
        {
            get { return stateHints; }
        }
    }
}
