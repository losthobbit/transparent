using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Events
{
    /// <summary>
    /// Implement for an event that happens when a user performs an action.
    /// </summary>
    public interface IUserActionEvent
    {
        /// <summary>
        /// Action to occur every time a user performs an action.
        /// </summary>
        /// <remarks>
        /// Currently all actions are synchronous.  This could be changed to improve performance if necessary.
        /// </remarks>
        /// <param name="userId">The ID of the user performing the action.</param>
        void Action(int userId);
    }
}
