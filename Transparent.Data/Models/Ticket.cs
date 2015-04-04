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
        protected abstract IEnumerable<TicketState> States { get; }

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
