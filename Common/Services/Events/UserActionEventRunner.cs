using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Events
{
    /// <summary>
    /// For triggering events on every user action
    /// </summary>
    public class UserActionEventRunner: IUserActionEventRunner
    {
        private List<IUserActionEvent> events;

        public UserActionEventRunner(IEnumerable<IUserActionEvent> events)
        {
            this.events = events.ToList();
        }

        /// <summary>
        /// Run events.
        /// </summary>
        /// <exception cref="AggregateException">Events threw exceptions.</exception>
        public void RunEvents(int userId)
        {
            var exceptions = new List<Exception>();
            foreach (var evt in events)
            {
                try
                {
                    evt.Action(userId);
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
    }
}
