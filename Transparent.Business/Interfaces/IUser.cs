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
    }
}
