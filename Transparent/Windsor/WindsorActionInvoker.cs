using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Windsor;

namespace Transparent.Windsor
{
    /// <summary>
    /// Injects services into action filters.
    /// </summary> 
    /// <remarks>
    /// From http://weblogs.asp.net/psteele/using-windsor-to-inject-dependencies-into-asp-net-mvc-actionfilters
    /// </remarks>
    public class WindsorActionInvoker : ControllerActionInvoker
    {
        readonly IWindsorContainer container;

        public WindsorActionInvoker(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override ActionExecutedContext InvokeActionMethodWithFilters(
                ControllerContext controllerContext,
                IList<IActionFilter> filters,
                ActionDescriptor actionDescriptor,
                IDictionary<string, object> parameters)
        {
            foreach (IActionFilter actionFilter in filters)
            {
                container.Kernel.InjectProperties(actionFilter);
            }
            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }
    }
}