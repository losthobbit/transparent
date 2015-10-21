using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public abstract class Voteable<TVote>
        where TVote : Vote
    {
        public virtual ICollection<TVote> Votes { get; set; }

        public Stance GetUserVote(int userId)
        {
            if (Votes == null)
                return Stance.Neutral;
            var vote = Votes.SingleOrDefault(v => v.FkUserId == userId);
            if (vote == null || vote.Points == 0)
                return Stance.Neutral;
            if (vote.Points > 0)
                return Stance.For;
            return Stance.Against;
        }

    }
}
