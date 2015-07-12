using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IUsersContextFactory
    {
        IUsersContext Create();
        void Release(IUsersContext usersContext);
    }
}
