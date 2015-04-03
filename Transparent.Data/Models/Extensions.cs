using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public static class Extensions
    {
        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).First();
        }

        public static KnowledgeLevel ToKnowledgeLevel(this int points, int competentPoints, int expertPoints)
        {
            return points < competentPoints
                ? KnowledgeLevel.Beginner
                : points < expertPoints
                    ? KnowledgeLevel.Competent
                    : KnowledgeLevel.Expert;
        }

        /// <summary>
        /// How the specified user ranked the ticket.
        /// </summary>
        public static Stance GetUserRank(this Ticket ticket, int userId)
        {
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.FkUserId == userId);
            if (rankRecord == null)
                return Stance.Neutral;
            return rankRecord.Up ? Stance.For : Stance.Against;
        }

        /// <summary>
        /// How the specified user voted for the ticket.
        /// </summary>
        public static Stance GetUserVote(this Ticket ticket, int userId)
        {
            var voteRecord = ticket.UserVotes.SingleOrDefault(vote => vote.FkUserId == userId);
            if (voteRecord == null)
                return Stance.Neutral;
            return voteRecord.For ? Stance.For : Stance.Against;
        }

        public static bool Any(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return false;
            foreach (var item in enumerable)
            {
                return true;
            }
            return false;
        }

        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return 0;
            int count = 0;
            foreach (var item in enumerable)
            {
                ++count;
            }
            return count;
        }
    }
}
