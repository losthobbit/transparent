using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Interfaces
{
    public interface ITechnicalConfiguration
    {
        /// <summary>
        /// The time between checks to see if a timed event needs to run.
        /// </summary>
        /// <remarks>
        /// To improve performance, set a longer time.
        /// To have more frequent checks, reduce time.
        /// </remarks>
        TimeSpan MinEventInterval { get; set; }

        /// <summary>
        /// For high traffic this could improve page loading time.
        /// </summary>
        /// <remarks>
        /// For low traffic it may be better to set this to false in order to do the updates before loading the page.
        /// </remarks>
        bool RunEventsAsync { get; set; }
    }
}
