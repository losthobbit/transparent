using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.Services
{
    /// <summary>
    /// Contains methods for getting user info for a user.
    /// </summary>
    /// <remarks>
    /// This must be transient, because injecting IUserContext is not safe.
    /// I might also want to consider creating the context at the start of a transaction in case I save an interrupted transaction.
    /// See http://stackoverflow.com/questions/10585478/one-dbcontext-per-web-request-why
    /// </remarks>
    public class User: IUser
    {
        private IUsersContext db;
        private readonly IConfiguration configuration;
        private readonly ITags tags;

        public User(IUsersContext db, IConfiguration configuration, ITags tags)
        {
            this.db = db;
            this.configuration = configuration;
            this.tags = tags;
        }

        public int GetPointsForTag(int userId, int tagId)
        {
            var userTag = db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
            return userTag == null ? 0 : userTag.TotalPoints;
        }

        public List<Tag> GetIncompetentParentsTags(int userId, int tagId)
        {
            var parents = tags.Find(tagId).Parents;
            return parents == null
                ? new List<Tag>()
                : parents.Where(p => GetPointsForTag(userId, p.Id) < configuration.PointsRequiredToBeCompetent).ToList();
        }
    }
}
