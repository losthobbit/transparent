using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Services;
using Transparent.Business.Tests.Helpers;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Common;

namespace Transparent.Business.Tests.Services
{
    [TestFixture]
    public class ProgressTicketsTests: BaseTests
    {
        ProgressTickets target;

        private Mock<IDataService> mockDataService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            mockDataService = new Mock<IDataService>();

            target = new ProgressTickets(() => UsersContext, mockDataService.Object, TestConfiguration);
        }

        #region ProgressTicketsWithVerifiedTags

        Ticket progressTicketsWithVerifiedTagsTicket;

        private void ArrangeProgressTicketsWithVerifiedTags()
        {
            progressTicketsWithVerifiedTagsTicket = TestData.JoesCriticalThinkingSuggestion;
            progressTicketsWithVerifiedTagsTicket.State = TicketState.Verification;
            progressTicketsWithVerifiedTagsTicket.TicketTags.ForEach(tag => tag.Verified = true);
            TestConfiguration.DelayAfterValidatingTags = TimeSpan.FromSeconds(10);
            progressTicketsWithVerifiedTagsTicket.ModifiedDate = DateTime.UtcNow.AddSeconds(-11);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_verified_ticket_changes_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            bool setNextStateCalled = false;
            bool setNextStateCalledAndSaved = false;
            Action updateTicket = () =>
            {
                setNextStateCalledAndSaved = setNextStateCalled;
            };
            updateTicket();
            UsersContext.SavedChanges += context => updateTicket();
            mockDataService.Setup(x => x.SetNextState(progressTicketsWithVerifiedTagsTicket))
                .Callback(() => { setNextStateCalled = true; });

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.IsTrue(setNextStateCalledAndSaved);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_unverified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.State = TicketState.Draft;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Draft, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_recently_modified_ticket_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.ModifiedDate = DateTime.UtcNow.AddSeconds(-8);

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_without_tags_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            var tags = progressTicketsWithVerifiedTagsTicket.TicketTags.ToList();
            progressTicketsWithVerifiedTagsTicket.TicketTags.Clear();
            foreach (var tag in tags)
            {
                TestData.UsersContext.TicketTags.Remove(tag);
            }

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        [Test]
        public void ProgressTicketsWithVerifiedTags_with_ticket_with_unverified_tag_does_not_change_ticket_state()
        {
            //Arrange
            ArrangeProgressTicketsWithVerifiedTags();
            progressTicketsWithVerifiedTagsTicket.TicketTags.Last().Verified = false;

            //Act
            target.ProgressTicketsWithVerifiedTags();

            //Assert
            Assert.AreEqual(TicketState.Verification, progressTicketsWithVerifiedTagsTicket.State);
        }

        #endregion ProgressTicketsWithVerifiedTags
    }
}
