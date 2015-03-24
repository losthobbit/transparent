using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Interfaces
{
    public interface IDataService
    {
        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void SetNextState(Ticket ticket);

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddPoints(UserPoint userPoint, UserTag userTag, int points);

        /// <summary>
        /// Adds points to the UserPoints and UserTags.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddPoints(IUsersContext db, IEnumerable<int> userId, int tagId, int testId, int points, PointReason reason);
    }
}
