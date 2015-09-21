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
    using Common.Interfaces;
    using ISecurity = Common.Interfaces.ISecurity;
    using IDataConfiguration = Data.Interfaces.IConfiguration;

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
        private readonly ITags tags;
        private readonly ISecurity security;
        private readonly IEmail emailService;
        private readonly IDataConfiguration configuration;

        public User(IUsersContext db, ITags tags, ISecurity security, IEmail emailService, IDataConfiguration configuration)
        {
            this.db = db;
            this.tags = tags;
            this.security = security;
            this.emailService = emailService;
            this.configuration = configuration;
        }

        public int GetPointsForTag(int userId, int tagId)
        {
            var userTag = GetUserTag(userId, tagId);
            return userTag == null ? 0 : userTag.TotalPoints;
        }

        public UserTag GetUserTag(int userId, int tagId)
        {
            return db.UserTags.SingleOrDefault(tag => tag.FkUserId == userId && tag.FkTagId == tagId);
        }

        public List<Tag> GetIncompetentParentsTags(int userId, int tagId)
        {
            var parents = tags.Find(tagId).Parents;
            return parents == null
                ? new List<Tag>()
                : parents.Where(p => GetPointsForTag(userId, p.Id) < p.CompetentPoints).ToList();
        }

        public void SetLastActionDate(int userId, DateTime dateTime)
        {
            db.UserProfiles.Single(user => user.UserId == userId).LastActionDate = dateTime;
            db.SaveChanges();
        }

        /// <summary>
        /// Creates temporary password and sends the reset password email.
        /// </summary>
        /// <remarks>
        /// Either username or email must be supplied.
        /// </remarks>
        /// <param name="username">The username of the account</param>
        /// <param name="email">The email address of the account</param>
        /// <exception cref="ArgumentException">username or email could not be found.</exception>
        public void ForgottenPassword(string username, string email)
        {
            var userProfile = String.IsNullOrWhiteSpace(username)
                ? db.UserProfiles.SingleOrDefault(user => user.Email == email)
                : db.UserProfiles.SingleOrDefault(user => user.UserName == username);

            if (userProfile == null)
                throw new ArgumentException("username or email could not be found");

            var temporaryPassword = CreateTemporaryPassword(userProfile);

            SendForgottenPasswordEmail(email, temporaryPassword);
        }

        private void SendForgottenPasswordEmail(string emailAddress, string temporaryPassword)
        {
            emailService.Send("Democratic Intelligence",
                String.Format(ResourceStrings.ForgottenPasswordEmailTemplate, 
                configuration.BaseSiteUrl + "/Account/ForgottenPassword?id=" +temporaryPassword),
                emailAddress);
        }

        private string CreateTemporaryPassword(UserProfile userProfile)
        {
            var temporaryPassword = Guid.NewGuid().ToString();

            db.TemporaryPasswords.Add(new TemporaryPassword
            {
                User = userProfile,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                Hash = security.Hash(temporaryPassword)
            });

            db.SaveChanges();

            return temporaryPassword;
        }
    }
}
