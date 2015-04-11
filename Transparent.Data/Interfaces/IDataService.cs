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
        /// Sets the state of the ticket to the next or specified state and sets the modified date.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void SetNextState(Ticket ticket, TicketState? specificState = null);

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddPoints(UserPoint userPoint, UserTag userTag, int points);

        /// <summary>
        /// Adds points to the UserPoints and UserTags for multiple users.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddPoints(IUsersContext db, IEnumerable<int> userId, int tagId, int points, PointReason reason, int? testId = null,
            int? ticketId = null);

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddPoints(IUsersContext db, int userId, int tagId, int points, PointReason reason, int? testId = null, int? ticketId = null);

        /// <summary>
        /// Adds application points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void AddApplicationPoints(IUsersContext db, int userId, int points, PointReason reason, int? ticketId = null);

        /// <summary>
        /// Adjusts the rank of the ticket based on the user's stance.
        /// </summary>
        /// <remarks>
        /// Ensures ticket rank and ticket user rank are adjusted together.
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        int SetRank(IUsersContext db, int ticketId, Stance ticketRank, int userId);

        /// <summary>
        /// Adjusts the votes of the ticket based on the user's stance.
        /// </summary>
        /// <remarks>
        /// Ensures that ticket votes (for and against) and ticket user vote are adjusted together.
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        void SetVote(Ticket ticket, Stance vote, int userId);
    }
}
