using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class UsersContainer :StatefulPagedList<UserProfile, UsersContainer>
    {
        public UsersContainer()
        {
        }

        public UsersContainer(IQueryable<UserProfile> users)
            : base(users)
        {
        }
    }
}
