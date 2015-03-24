using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data.Services
{
    /// <summary>
    /// For high level data operations.
    /// </summary>
    /// <remarks>
    /// Designed to be used as a singleton.
    /// Methods do not call DbContext.SaveChanges.
    /// </remarks>
    public class DataService : IDataService
    {
        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void SetNextState(Ticket ticket)
        {
            var state = ticket.NextState;
            if (state == null)
                throw new NotSupportedException("Ticket does not have a next state");
            ticket.State = ticket.NextState.Value;
            ticket.ModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void AddPoints(UserPoint userPoint, UserTag userTag, int points)
        {
            userPoint.Quantity += points;
            userTag.TotalPoints += points;
        }

        /// <summary>
        /// Adds points to the UserPoints and UserTags.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        public void AddPoints(IUsersContext db, IEnumerable<int> userId, int tagId, int testId, int points, PointReason reason)
        {
            foreach (var user in userId)
            {
                AddPoints(db, user, tagId, testId, points, reason);
            }
        }

        /// <summary>
        /// Adds points to the UserPoint and UserTag.
        /// </summary>
        /// <remarks>
        /// Does not call DbContext.SaveChanges.
        /// </remarks>
        private void AddPoints(IUsersContext db, int userId, int tagId, int testId, int points, PointReason reason)
        {
            var userPoint = db.UserPoints.SingleOrDefault(point => point.FkUserId == userId && point.FkTagId == tagId && point.FkTestId == testId);
            if (userPoint == null)
            {
                userPoint = new UserPoint { FkUserId = userId, FkTagId = tagId, FkTestId = testId, Reason = reason };
                db.UserPoints.Add(userPoint);
            }

            var userTag = db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
            if (userTag == null)
            {
                userTag = new UserTag { FkTagId = tagId, FkUserId = userId };
                db.UserTags.Add(userTag);
            }
            AddPoints(userPoint, userTag, points);
        }
    }
}