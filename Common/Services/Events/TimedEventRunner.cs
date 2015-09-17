using Common.Interfaces;
using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services.Events
{
    /// <summary>
    /// For triggering events periodically.
    /// </summary>
    public class TimedEventRunner: ITimedEventRunner
    {
        private readonly object _lock = new object();       
        
        private List<ITimedEvent> events;

        public TimedEventRunner(IEnumerable<ITimedEvent> events)
        {
            this.events = events.ToList();
        }

        /// <summary>
        /// Run events which are ready, in a separate thread.
        /// </summary>
        public void RunEventsAsync()
        {
            // Initial check so that I don't have to lock unless necessary
            var readyEvents = GetReadyEvents(events);

            if (readyEvents.Any())
            {
                new Task(() => RunEvents(readyEvents)).Start();
            }
        }

        /// <summary>
        /// Run events which are ready.
        /// </summary>
        /// <exception cref="AggregateException">Events threw exceptions.</exception>
        public void RunEvents(IEnumerable<ITimedEvent> readyEvents = null)
        {
            var eventsToRun = GetEventsToRun(readyEvents ?? events);

            var exceptions = new List<Exception>();
            foreach (var evt in eventsToRun)
            {
                try
                {
                    evt.Action();
                }
                // Give all events a chance to run.
                catch(Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public void AddEvent(ITimedEvent evt)
        {
            events.Add(evt);
        }

        public void RemoveEvent(ITimedEvent evt)
        {
            events.Remove(evt);
        }

        /// <summary>
        /// Returns an array of events that are ready to run.
        /// </summary>
        private ITimedEvent[] GetReadyEvents(IEnumerable<ITimedEvent> events)
        {
            return events.Where(evt => evt.LastRun + evt.Interval <= DateTime.Now).ToArray();
        }

        /// <summary>
        /// Thread safe way of updating event dates and returning those that are going to be run.
        /// </summary>
        private ITimedEvent[] GetEventsToRun(IEnumerable<ITimedEvent> readyEvents)
        {
            lock (_lock)
            {
                var eventsToRun = GetReadyEvents(readyEvents);
                foreach (var evt in eventsToRun)
                {
                    evt.LastRun = DateTime.Now;
                }
                return eventsToRun;
            }
        }
    }
}
