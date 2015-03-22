using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Business.Interfaces
{
    public interface IUser
    {
        int GetPointsForTag(int userId, int tagId);
    }
}
