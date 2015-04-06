using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;
using Transparent.Business.ViewModels;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.Services
{
    /// <summary>
    /// Contains methods for getting and setting volunteer information.
    /// </summary>
    /// <remarks>
    /// This must be transient, because injecting IUserContext is not safe.
    /// I might also want to consider creating the context at the start of a transaction in case I save an interrupted transaction.
    /// See http://stackoverflow.com/questions/10585478/one-dbcontext-per-web-request-why
    /// </remarks>
    public class Volunteers: IVolunteers
    {
        private readonly IUsersContext db;

        public Volunteers(IUsersContext db)
        {
            this.db = db;
        }

        public VolunteerViewModel GetVolunteer(string username, bool hasVolunteerRole)
        {
            return new VolunteerViewModel
            {
                Volunteer = hasVolunteerRole,
                Services = db.UserProfiles.Single(user => user.UserName == username).Services
            };
        }

        public void SetServices(string username, string services)
        {
            db.UserProfiles.Single(user => user.UserName == username).Services = services;
            db.SaveChanges();
        }

        public UsersContainer GetVolunteers(UsersContainer filter)
        {
            return filter.Initialize
            (
                filter.ApplyFilter
                (
                    from volunteer in db.UserProfiles
                    join userInRole in db.UsersInRoles on volunteer.UserId equals userInRole.UserId
                    join role in db.Roles on userInRole.RoleId equals role.RoleId
                    where role.RoleName == "volunteer"
                    select volunteer
                ).OrderBy(volunteer => volunteer.UserName)     
            );
        }
    }
}
