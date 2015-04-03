using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Tests.Common;

namespace Transparent.Data.Tests.Helpers
{
    public class FakeTicketDbSet: FakeDbSet<Ticket>
    {
        public override Ticket Find(params object[] keyValues)
        {
            return _data.SingleOrDefault(ticket => ticket.Id == (int)keyValues[0]);
        }
    }
}
