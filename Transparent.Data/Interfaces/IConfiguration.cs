using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IConfiguration
    {
        string CurrentSubGoal { get; set; }

        int PointsRequiredBeforeDeductingPoints { get; set; }
        int PointsToDeductWhenStartingTest { get; set; }
        int PointsForPassingATest { get; set; }
        int MarkersRequiredPerTest { get; set; }
        int PointsMarkersGainForAgreeingATestResult { get; set; }
        int PointsMarkersLoseForDisagreeingATestResult { get; set; }

        int PointsRequiredToBeCompetent { get; set; }
        int PointsRequiredToBeAnExpert { get; set; }

        /// <summary>
        /// Delay after tags have been validated before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for tags to be added.
        /// </remarks>
        TimeSpan DelayAfterValidatingTags { get; set; }
    }
}
