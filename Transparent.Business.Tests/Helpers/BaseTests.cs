using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Business.Tests.Helpers
{
    public abstract class BaseTests
    {
        protected Fixture Fixture;
        protected TestData TestData;
        protected IConfiguration TestConfiguration;
        protected FakeUsersContext UsersContext;

        public virtual void SetUp()
        {
            Fixture = new Fixture();
            TestData = TestData.Create();
            UsersContext = TestData.UsersContext;
            TestConfiguration = new TestConfig();
        }

        protected void AssertModifiedDateSet(DateTime modifiedDate)
        {
            Assert.GreaterOrEqual(modifiedDate, DateTime.UtcNow.AddSeconds(-3));
            Assert.LessOrEqual(modifiedDate, DateTime.UtcNow);
        }
    }
}
