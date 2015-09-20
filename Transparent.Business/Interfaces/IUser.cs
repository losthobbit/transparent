using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Business.Interfaces
{
    public interface IUser
    {
        int GetPointsForTag(int userId, int tagId);
        UserTag GetUserTag(int userId, int tagId);
        List<Tag> GetIncompetentParentsTags(int userId, int tagId);
        void SetLastActionDate(int userId, DateTime dateTime);

        /// <summary>
        /// Creates temporary password and sends the reset password email.
        /// </summary>
        /// <remarks>
        /// Either username or email must be supplied.
        /// </remarks>
        /// <param name="username">The username of the account</param>
        /// <param name="email">The email address of the account</param>
        /// <exception cref="ArgumentException">username or email could not be found.</exception>
        void ForgottenPassword(string username, string email);
    }
}
