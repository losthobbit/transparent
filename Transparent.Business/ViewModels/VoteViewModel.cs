using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public class VoteViewModel: BaseVoteViewModel
    {
        public int TicketId { get; set; }
        public int VotesFor { get; set; }
        public int VotesAgainst { get; set; }
        public Stance NewVote { get; set; }

        // TODO: Check if this is correct
        public override int Id
        {
            get
            {
                return TicketId;
            }
            set
            {
                TicketId = value;
            }
        }
    }
}
