using Common.Enums;
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
        private readonly IDataService dataService;
        private readonly IConfiguration configuration;

        public Volunteers(IUsersContext db, IDataService dataService, IConfiguration configuration)
        {
            this.db = db;
            this.dataService = dataService;
            this.configuration = configuration;
        }

        public VolunteerViewModel GetVolunteer(string username, bool hasVolunteerRole)
        {
            return new VolunteerViewModel
            {
                Volunteer = hasVolunteerRole,
                Services = db.UserProfiles.Single(user => user.UserName == username).Services
            };
        }

        public void Set(string username, string services, Relative changedVolunteerStatus)
        {
            var user = db.UserProfiles.Single(u => u.UserName == username);
            user.Services = services;
            if (changedVolunteerStatus != Relative.EqualTo)
            {
                dataService.AddApplicationPoints(db, user.UserId, configuration.DiPointsForVolunteering * (int)changedVolunteerStatus,
                    PointReason.Volunteered);
            }
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
