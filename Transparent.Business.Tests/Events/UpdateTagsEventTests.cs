using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Events;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.Data.Tests.Helpers;

namespace Transparent.Business.Tests.Events
{
    [TestFixture]
    public class UpdateTagsEventTests: BaseTests
    {
        private UpdateTagsEvent target;

        private Mock<Common.Interfaces.IConfiguration> mockConfiguration;
        private Mock<IUsersContextFactory> mockUsersContextFactory;
        private Mock<IDataService> mockDataService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mockConfiguration = new Mock<Common.Interfaces.IConfiguration>();
            mockConfiguration.Setup(x => x.GetValue("UpdateTagsEventInterval")).Returns("00:01:00");

            mockUsersContextFactory = new Mock<IUsersContextFactory>();
            mockUsersContextFactory.Setup(x => x.Create()).Returns(UsersContext);

            mockDataService = new Mock<IDataService>();

            target = new UpdateTagsEvent(mockConfiguration.Object, MockTags.Object, mockUsersContextFactory.Object, TestConfiguration,
                mockDataService.Object);
        }

        #region UpdateCompetencyLevels

        private Tag updateCompetencyLevelsTag;

        private void ArrangeUpdateCompetencyLevels(int[] userPoints)
        {
            // Remove existing users
            //foreach (var user in TestData.UsersContext.UserProfiles.ToList())
            //    TestData.UsersContext.UserProfiles.Remove(user);

            // Create an active user for each point
            var activeUsers = new List<UserProfile>();
            updateCompetencyLevelsTag = TestData.UnusedTag;
            int userId = 0;
            foreach (var userPoint in userPoints)
            {
                var user = new UserProfile
                {
                    UserId = userId,
                    Tags = new List<UserTag>()
                };
                TestData.UsersContext.UserProfiles.Add(user);
                activeUsers.Add(user);
                if (userPoint > 0)
                {
                    var userTag = new UserTag
                    {
                        FkTagId = updateCompetencyLevelsTag.Id,
                        Tag = updateCompetencyLevelsTag,
                        FkUserId = userId,
                        User = user,
                        TotalPoints = userPoint
                    };
                    user.Tags.Add(userTag);
                    TestData.UsersContext.UserTags.Add(userTag);
                }
                userId++;
            }
            mockDataService.Setup(x => x.GetActiveUsers(It.IsAny<IUsersContext>())).Returns(activeUsers.AsQueryable);
        }

        [TestCase(50, 2, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(26, 1, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(25, 1, 100, new[] { 0, 0, 0, 10 }, 10)]
        [TestCase(25, 2, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(25, 2, 100, new[] { 1, 2 }, 1)]
        [TestCase(25, 3, 100, new[] { 1, 2 }, 0)]
        [TestCase(25, 3, 100, new int[] { }, 0)]
        [TestCase(60, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 5)]
        [TestCase(61, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 4)]
        [TestCase(59, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 5)]
        [TestCase(59, 1,  35, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 4)]
        [TestCase(59, 1,  30, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 3)]
        public void UpdateCompetencyLevels_sets_competent_points_to_expected_values(
            int minPercentCompetents, int minCompetents, int competentPercOfHighest, int[] userPoints, int expectedCompetentPoints)
        {
            //Arrange
            ArrangeUpdateCompetencyLevels(userPoints);
            TestConfiguration.CompetentPercentOfHighestScore = competentPercOfHighest;
            TestConfiguration.MinPercentCompetents = minPercentCompetents;
            TestConfiguration.MinCompetents = minCompetents;
            var postSaveAssert = TestData.UsersContext.PostSaveAssert(() =>
                updateCompetencyLevelsTag.CompetentPoints == expectedCompetentPoints);

            //Act
            target.UpdateCompetencyLevels();

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        [TestCase(50, 2, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(26, 1, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(25, 1, 100, new[] { 0, 0, 0, 10 }, 10)]
        [TestCase(25, 2, 100, new[] { 0, 0, 0, 10 }, 0)]
        [TestCase(25, 2, 100, new[] { 1, 2 }, 1)]
        [TestCase(25, 3, 100, new[] { 1, 2 }, 0)]
        [TestCase(25, 3, 100, new int[] { }, 0)]
        [TestCase(60, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 5)]
        [TestCase(61, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 4)]
        [TestCase(59, 1, 100, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 5)]
        [TestCase(59, 1,  35, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 4)]
        [TestCase(59, 1,  30, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 3)]
        public void UpdateCompetencyLevels_sets_expert_points_to_expected_values(
            int minPercentExperts, int minExperts, int expertPercOfHighest, int[] userPoints, int expectedExpertPoints)
        {
            //Arrange
            ArrangeUpdateCompetencyLevels(userPoints);
            TestConfiguration.ExpertPercentOfHighestScore = expertPercOfHighest;
            TestConfiguration.MinPercentExperts = minPercentExperts;
            TestConfiguration.MinExperts = minExperts;
            var postSaveAssert = TestData.UsersContext.PostSaveAssert(() =>
                updateCompetencyLevelsTag.ExpertPoints == expectedExpertPoints);

            //Act
            target.UpdateCompetencyLevels();

            //Assert
            postSaveAssert.AssertIsTrue();
        }

        #endregion UpdateCompetencyLevels
    }
}
