using Common.Interfaces.Events;
using Common.Services.Events;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Tests.Services.Events
{
    [TestFixture]
    public class EventRunnerTests
    {
        private EventRunner target;

        private Mock<IEvent> evt;

        [SetUp]
        public void SetUp()
        {
            target = new EventRunner(new List<IEvent>());

            evt = new Mock<IEvent>();
            evt.SetupGet(x => x.LastRun).Returns(DateTime.Now.AddSeconds(-10));
            evt.SetupGet(x => x.Interval).Returns(TimeSpan.FromSeconds(9));
            target.AddEvent(evt.Object);
        }

        #region RunEvents

        [Test]
        public void RunEvents_with_ready_event_runs_event()
        {
            //Act
            target.RunEvents();

            //Assert
            evt.Verify(x => x.Action(), Times.Once);
        }      

        [Test]
        public void RunEvents_with_not_ready_event_runs_event()
        {
            //Arrange
            evt.SetupGet(x => x.LastRun).Returns(DateTime.Now);

            //Act
            target.RunEvents();

            //Assert
            evt.Verify(x => x.Action(), Times.Never);
        }

        [Test]
        [ExpectedException(typeof(AggregateException))]
        public void RunEvents_with_ready_event_which_throws_exception_throws_AggregateException()
        {
            //Arrange
            evt.Setup(x => x.Action()).Throws<Exception>();

            //Act
            target.RunEvents();
        }

        #endregion RunEvents

        #region RunEventsAsync

        [Test]
        public void RunEventsAsync_with_ready_event_runs_event()
        {
            //Act
            target.RunEventsAsync();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            //Assert
            evt.Verify(x => x.Action(), Times.Once);
        }

        [Test]
        public void RunEventsAsync_with_not_ready_event_runs_event()
        {
            //Arrange
            evt.SetupGet(x => x.LastRun).Returns(DateTime.Now);

            //Act
            target.RunEventsAsync();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            //Assert
            evt.Verify(x => x.Action(), Times.Never);
        }

        #endregion RunEventsAsync

    }
}
