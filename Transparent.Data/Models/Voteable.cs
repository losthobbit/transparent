using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public abstract class Voteable<TVote>
        where TVote : Vote, new()
    {
        public virtual ICollection<TVote> Votes { get; set; }

        /// <summary>
        /// For performance and to make querying easier.
        /// </summary>
        [Display(Name = "Points")]
        public int TotalPoints { get; set; }

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

        /// <summary>
        /// Sets the user's vote and updates the total.
        /// </summary>
        /// <param name="userId">ID of the user whose points should be set.</param>
        /// <param name="points">Replacement points.</param>
        public void SetUserVote(int userId, int points)
        {
            if (Votes == null)
                Votes = new List<TVote>();
            var vote = Votes.SingleOrDefault(v => v.FkUserId == userId);
            if (points == 0)
            {
                if (vote != null)
                {
                    TotalPoints -= vote.Points;
                    Votes.Remove(vote);
                }
            }
            else
            {
                if (vote == null)
                {
                    vote = new TVote();
                    TotalPoints += points;
                    Votes.Add(vote);
                }
                else
                {
                    TotalPoints += (points - vote.Points);
                }
                vote.FkUserId = userId;
                vote.Points = points;
            }
        }
    }
}
