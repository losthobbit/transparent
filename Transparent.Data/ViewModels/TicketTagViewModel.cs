using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.ViewModels
{
    public class TicketTagViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public bool UserMayVerify { get; set; }
    }

}
