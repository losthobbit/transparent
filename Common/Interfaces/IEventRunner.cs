using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    /// <summary>
    /// For triggering events periodically.
    /// </summary>
    public interface IEventRunner
    {
        void RunEventsAsync();
        void RunEvents();

        void AddEvent(Action action);
        void RemoveEvent(Action action);
    }
}
