using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Queries;
using Transparent.Data.Tests.Helpers;

namespace Transparent.Data.Tests.Queries
{
    [TestClass]
    public class TicketsTests
    {
        private Tickets target;

        private IUsersContext usersContext;

        [TestInitialize()]
        public void Initialize()
        {
            usersContext = TestData.CreateUsersContext();
            target = new Tickets(usersContext);
        }

        #region MyQueue

        [TestMethod]
        public void MyQueue_with_ticket_and_user_with_same_tag_returns_ticket()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void MyQueue_with_ticket_and_user_without_same_tag_does_not_return_ticket()
        {
            throw new NotImplementedException();
        }

        #endregion MyQueue
    }
}
