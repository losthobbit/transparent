using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IThresholds
    {
        /// <summary>
        /// The number of points on a tag to indicate that it is 100% accepted.
        /// </summary>
        int FullAcceptanceThreshold { get; set; }

        /// <summary>
        /// The minimum number of points on a tag to make the tag count.
        /// </summary>
        int NotAcceptedThreshold { get; set; }
    }
}
