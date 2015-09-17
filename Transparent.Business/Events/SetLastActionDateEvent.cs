using Common.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.Interfaces;

namespace Transparent.Business.Events
{
    /// <summary>
    /// Sets the last action date every time a user performs an action.
    /// </summary>
    public class SetLastActionDateEvent: UserActionEvent
    {
        private IUserFactory userServiceFactory;

        public SetLastActionDateEvent(IUserFactory userServiceFactory)
        {
            this.userServiceFactory = userServiceFactory;
        }

        public override void Action(int userId)
        {
            if (userId < 0)
                return;

            userServiceFactory.Create().SetLastActionDate(userId, DateTime.UtcNow);
        }
    }
}
