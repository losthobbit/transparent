using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Events
{
    /// <summary>
    /// For triggering events on every user action
    /// </summary>
    public interface IUserActionEventRunner
    {
        /// <summary>
        /// Run events.
        /// </summary>
        /// <exception cref="AggregateException">Events threw exceptions.</exception>
        void RunEvents(int userId);
    }
}
