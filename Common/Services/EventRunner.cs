using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services
{
    /// <summary>
    /// For triggering events periodically.
    /// </summary>
    public class EventRunner: IEventRunner
    {
        private List<Action> events = new List<Action>();

        public void RunEventsAsync()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                RunEvents();
            }).Start();
        }

        public void RunEvents()
        {
            foreach (var action in events)
            {
                action();
            }
        }

        public void AddEvent(Action action)
        {
            events.Add(action);
        }

        public void RemoveEvent(Action action)
        {
            events.Remove(action);
        }
    }
}
