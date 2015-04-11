using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Tests.Helpers
{
    public abstract class BaseTests
    {
        protected Fixture Fixture;
        protected TestData TestData;
        protected IConfiguration TestConfiguration;
        protected FakeUsersContext UsersContext;
        protected Mock<ITags> MockTags;

        public virtual void SetUp()
        {
            Fixture = new Fixture();
            TestData = TestData.Create();
            UsersContext = TestData.UsersContext;
            TestConfiguration = new TestConfig();
            MockTags = new Mock<ITags>();
            MockTags.Setup(x => x.Find(It.IsAny<int>()))
                .Returns<int>(id => TestData.UsersContext.Tags.Single(tag => tag.Id == id));
            MockTags.SetupGet(x => x.ApplicationTag)
                .Returns(TestData.UsersContext.Tags.Single(tag => tag.Name == Constants.ApplicationName));
        }

        protected void AssertModifiedDateSet(DateTime modifiedDate)
        {
            Assert.GreaterOrEqual(modifiedDate, DateTime.UtcNow.AddSeconds(-3));
            Assert.LessOrEqual(modifiedDate, DateTime.UtcNow);
        }
    }
}
