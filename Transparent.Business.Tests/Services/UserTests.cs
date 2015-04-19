using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Services;
using Transparent.Data.Interfaces;
using Transparent.Data.Tests.Helpers;

namespace Transparent.Business.Tests.Services
{
    [TestFixture]
    public class UserTests: BaseTests
    {
        private User target;

        private Mock<ITags> mockTags;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mockTags = new Mock<ITags>();

            target = new User(TestData.UsersContext, TestConfiguration, mockTags.Object);
        }

        #region GetIncompetentParentsTags

        [Test]
        public void GetIncompetentParentsTags_where_the_user_is_competent_in_the_parent_tag_does_not_return_tag()
        {
            // Arrange
            var tag = TestData.BungeeJumpingTag;
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints + new Random().Next(2);
            var userId = TestData.Stephen.UserId;
            mockTags.Setup(x => x.Find(tag.Id)).Returns(tag);

            // Act
            var actualIncompetentParentsTags = target.GetIncompetentParentsTags(userId, tag.Id);

            // Assert
            Assert.IsFalse(actualIncompetentParentsTags.Contains(TestData.CriticalThinkingTag));
        }

        [Test]
        public void GetIncompetentParentsTags_where_the_user_is_incompetent_in_the_parent_tag_returns_tag()
        {
            // Arrange
            var tag = TestData.BungeeJumpingTag;
            TestData.StephensCriticalThinkingTag.TotalPoints = TestData.CriticalThinkingTag.CompetentPoints - 1;
            var userId = TestData.Stephen.UserId;
            mockTags.Setup(x => x.Find(tag.Id)).Returns(tag);

            // Act
            var actualIncompetentParentsTags = target.GetIncompetentParentsTags(userId, tag.Id);

            // Assert
            Assert.IsTrue(actualIncompetentParentsTags.Contains(TestData.CriticalThinkingTag));
        }

        [Test]
        public void GetIncompetentParentsTags_where_the_user_doesnt_have_the_parent_tag_returns_tag()
        {
            // Arrange
            var tag = TestData.BungeeJumpingTag;
            TestData.Stephen.Tags.Remove(TestData.StephensCriticalThinkingTag);
            TestData.UsersContext.UserTags.Remove(TestData.StephensCriticalThinkingTag);
            var userId = TestData.Stephen.UserId;
            mockTags.Setup(x => x.Find(tag.Id)).Returns(tag);

            // Act
            var actualIncompetentParentsTags = target.GetIncompetentParentsTags(userId, tag.Id);

            // Assert
            Assert.IsTrue(actualIncompetentParentsTags.Contains(TestData.CriticalThinkingTag));
        }

        #endregion GetIncompetentParentsTags
    }
}
