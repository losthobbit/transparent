using Common.Interfaces;
using Common.Interfaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.Interfaces;

namespace Transparent.Filters
{
    /// <summary>
    /// Allows the code to trigger events that need to occur periodically.
    /// </summary>
    /// <remarks>
    /// Obviously this is not the ideal way to do things.  A better way, for example would be to use a Windows service.
    /// Since I'm hosting this in a very simple environment, I don't think I have much choice.  So this is how I'll do
    /// it for now, and if it gets hosted in a different environment we can consider doing it a different way.
    /// </remarks>
    public class PeriodicFilterAttribute : ActionFilterAttribute   
    {
        public ITechnicalConfiguration TechnicalConfiguration { get; set; }
        public IEventRunner EventRunner { get; set; }

        private readonly object _lock = new object();

        private DateTime lastRun = DateTime.MinValue;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var doEvent = false;

            // Initial check so that I don't have to lock unless necessary
            if (lastRun + TechnicalConfiguration.MinEventInterval <= DateTime.Now)
            {
                lock (_lock)
                {
                    // In case it changed since the lock
                    if (lastRun + TechnicalConfiguration.MinEventInterval <= DateTime.Now)
                    {
                        lastRun = DateTime.Now;
                        // Release the lock ASAP, rather than creating the thread inside the lock
                        doEvent = true;
                    }
                }
            }

            if (doEvent)
            {
                if(TechnicalConfiguration.RunEventsAsync)
                    // Better for high traffic to make the page load faster
                    EventRunner.RunEventsAsync();
                else
                    // Better for low traffic so that updates occur before page loads
                    EventRunner.RunEvents();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}