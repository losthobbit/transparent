using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public class VoteViewModel
    {
        public int TicketId { get; set; }
        public int VotesFor { get; set; }
        public int VotesAgainst { get; set; }
        public Stance UserVote { get; set; }
        public Stance NewVote { get; set; }
        public bool UserMayVote { get; set; }
    }
}
