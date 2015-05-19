using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public abstract class Ticket : BaseTicket
    {
        protected Ticket()
        {
            Initialize();
        }

        protected Ticket(int id, int rank)
            : base(id, rank)
        {
            Initialize();
        }

        private void Initialize()
        {
            State = StartingState;
        }

        /// <summary>
        /// The user that created the ticket
        /// </summary>
        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        public virtual ICollection<TicketUserRank> UserRanks { get; set; }

        public static Ticket Create(TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return new Question();
                case TicketType.Suggestion: return new Suggestion();
                case TicketType.Test: return new Test();
            }
            throw new NotSupportedException("Unknown ticket type");
        }

        public virtual ICollection<TicketUserVote> UserVotes { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }

        /// <summary>
        /// The user that the ticket is assigned to (or last changed the state of the ticket)
        /// </summary>
        /// <remarks>
        /// Not to be set without adding to History.
        /// </remarks>
        [ForeignKey("AssignedUser")]
        public int? FkAssignedUserId { get; set; }
        public virtual UserProfile AssignedUser { get; set; }

        [Required]
        public int VotesFor { get; set; }

        [Required]
        public int VotesAgainst { get; set; }

        /// <summary>
        /// There can potentially be more than one state, e.g. after Voting.
        /// This property should only be used when a state can only have one
        /// next state.
        /// </summary>
        [NotMapped]
        public TicketState? NextState
        {
            get
            {
                var found = false;
                foreach (var state in States)
                {
                    if (found)
                        return state;
                    if (state == this.State)
                        found = true;
                }
                // last state or this.State not found
                return null;
            }
        }

        [NotMapped]
        protected IEnumerable<TicketState> States { get { return StateHints.Keys; } }

        [NotMapped]
        public override Hint StateHint
        {
            get 
            {
                return StateHints[State];
            }
        }

        /// <summary>
        /// Key is state, value is hint associated with that state
        /// </summary>
        [NotMapped]
        protected abstract Dictionary<TicketState, Hint> StateHints { get; }

        [NotMapped]
        protected TicketState StartingState { get { return States.First(); } }

        /// <summary>
        /// Attempt to set state.  If the state is not valid for this type, it could be redirected.
        /// </summary>
        public virtual void TrySetState(TicketState state)
        {
            State = state;
        }
    }
}
