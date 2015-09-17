using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Interfaces;
using WebMatrix.WebData;

namespace Transparent.Filters
{
    /// <summary>
    /// Performs functions required when an authenticated user triggers an action.
    /// </summary>
    public class UserActionFilterAttribute : ActionFilterAttribute
    {
        public IUserActionEventRunner userActionEventRunner { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            userActionEventRunner.RunEvents(WebSecurity.CurrentUserId);

            base.OnActionExecuting(filterContext);
        }
    }
}