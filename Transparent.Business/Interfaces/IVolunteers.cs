using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels;
using Transparent.Data.Models;

namespace Transparent.Business.Interfaces
{
    public interface IVolunteers
    {
        VolunteerViewModel GetVolunteer(string username, bool hasVolunteerRole);
        void Set(string username, string services, Relative changedVolunteerStatus);
        UsersContainer GetVolunteers(UsersContainer filter);
    }
}
