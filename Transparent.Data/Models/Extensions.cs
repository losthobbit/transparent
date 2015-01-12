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

        public static TicketRank GetTicketRank(this Ticket ticket, string userName)
        {
            var rankRecord = ticket.UserRanks.SingleOrDefault(rank => rank.User.UserName == userName);
            if (rankRecord == null)
                return TicketRank.NotRanked;
            return rankRecord.Up ? TicketRank.Up : TicketRank.Down;
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
