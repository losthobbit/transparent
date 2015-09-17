using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Events
{
    public class UserActionEvent: IUserActionEvent
    {
        public virtual void Action(int userId)
        {
        }
    }
}
