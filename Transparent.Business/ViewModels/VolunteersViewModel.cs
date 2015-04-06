using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public class VolunteersViewModel : List<VolunteerViewModel>
    {
        public VolunteersViewModel(): base()
        {

        }

        public VolunteersViewModel(IEnumerable<VolunteerViewModel> collection): base(collection)
        {

        }
    }
}
