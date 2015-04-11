using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Transparent.Business.Interfaces;
using Transparent.Business.ViewModels;
using Transparent.Data;
using Transparent.Data.Models;
using WebMatrix.WebData;

namespace Transparent.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly IVolunteers volunteers;

        public VolunteerController(IVolunteers volunteers)
        {
            this.volunteers = volunteers;
        }

        [HttpGet]
        public ActionResult Volunteer()
        {
            return View(volunteers.GetVolunteer(User.Identity.Name, User.IsInRole(Constants.VolunteerRole)));
        }

        [HttpPost]
        public ActionResult Volunteer(VolunteerViewModel volunteerViewModel)
        {
            bool wasVolunteer = User.IsInRole(Constants.VolunteerRole);
            Relative changedVolunteerStatus = Relative.EqualTo;
            if (volunteerViewModel.Volunteer && !wasVolunteer)
            {
                Roles.AddUserToRole(User.Identity.Name, Constants.VolunteerRole);
                changedVolunteerStatus = Relative.GreaterThan;
            }
            else
                if (!volunteerViewModel.Volunteer && wasVolunteer)
                {
                    Roles.RemoveUserFromRole(User.Identity.Name, Constants.VolunteerRole);
                    changedVolunteerStatus = Relative.LessThan;
                }

            volunteers.Set(User.Identity.Name, volunteerViewModel.Services, changedVolunteerStatus);

            return View(volunteerViewModel);
        }

        public ActionResult Volunteers(UsersContainer filter)
        {
            return View(volunteers.GetVolunteers(filter));
        }
    }
}
