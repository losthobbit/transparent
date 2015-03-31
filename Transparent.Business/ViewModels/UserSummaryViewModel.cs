using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Business.ViewModels
{
    public class UserSummaryViewModel
    {
        public string Username { get; set; }
        public ICollection<UserTagViewModel> Tags { get; set; }
    }
}
