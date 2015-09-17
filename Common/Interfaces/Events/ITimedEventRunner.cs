using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Events
{
    /// <summary>
    /// For triggering events periodically.
    /// </summary>
    public interface ITimedEventRunner
    {
        void RunEventsAsync();
        /// <exception cref="AggregateException">Events threw exceptions.</exception>
        void RunEvents(IEnumerable<ITimedEvent> readyEvents = null);

        void AddEvent(ITimedEvent evt);
        void RemoveEvent(ITimedEvent evt);
    }
}
