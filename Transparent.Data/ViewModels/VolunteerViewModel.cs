using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Data.ViewModels
{
    [MetadataType(typeof(IVolunteer))]
    public class VolunteerViewModel : IVolunteer
    {
        public bool Volunteer { get; set; }
        public string Services { get; set; }
    }
}
