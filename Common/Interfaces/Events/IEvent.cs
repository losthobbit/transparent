using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Events
{
    public interface IEvent
    {
        DateTime LastRun { get; set; }
        TimeSpan Interval { get; set; }
        void Action();
    }
}
