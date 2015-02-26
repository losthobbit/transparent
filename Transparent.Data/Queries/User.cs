using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Queries
{
    /// <summary>
    /// Contains methods for getting user info for a user.
    /// </summary>
    /// <remarks>
    /// There's no need to split this into a business and data service, unless a different data source is required.
    /// Probably should not be a singleton.  I'm not sure that injecting IUserContext is safe... I don't know if that guarantees a unique context per thread,
    /// nor whether that matters or not.  I might also want to consider creating the context at the start of a transaction in case I save an interrupted
    /// transaction.
    /// </remarks>
    public class User: IUser
    {
        private IUsersContext db;

        private readonly IConfiguration configuration;

        public User(IUsersContext db, IConfiguration configuration)
        {
            this.db = db;

            this.configuration = configuration;
        }

        public int GetPointsForTag(int userId, int tagId)
        {
            var userTag = db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
            return userTag == null ? 0 : userTag.TotalPoints;
        }
    }
}
