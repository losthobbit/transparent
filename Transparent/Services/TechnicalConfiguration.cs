using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Interfaces;

namespace Transparent.Services
{
    public class TechnicalConfiguration : ITechnicalConfiguration
    {
        /// <summary>
        /// The time between checks to see if a timed event needs to run.
        /// </summary>
        /// <remarks>
        /// To improve performance, set a longer time.
        /// To have more frequent checks, reduce time.
        /// </remarks>
        public TimeSpan MinEventInterval { get; set; }

        public TechnicalConfiguration(Common.Interfaces.IConfiguration configuration)
        {
            MinEventInterval = TimeSpan.Parse(configuration.GetValue("MinEventInterval"));
        }
    }
}