using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Data.Tests.Helpers
{
    public class TestData
    {
        public static IUsersContext CreateUsersContext()
        {
            return new FakeUsersContext
            {
                Tickets =
                {
                    new Ticket { }
                }
            };
        }
    }
}
