using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Events
{
    public abstract class Event: IEvent
    {
        protected Event(TimeSpan interval)
        {
            Interval = interval;
            LastRun = DateTime.MinValue;
        }

        public DateTime LastRun { get; set; }
        public TimeSpan Interval { get; set; }

        public abstract void Action();
    }
}
