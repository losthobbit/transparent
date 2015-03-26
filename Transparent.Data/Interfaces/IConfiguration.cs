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
        /// The maximum position (based on highest ranked within a state) that a ticket can
        /// be in before being allowed to advance to the next state via a timed event.
        /// </summary>
        /// <remarks>
        /// This prevents low ranked tickets from progressing faster than high ranked tickets.
        /// </remarks>
        int MaxPositionToAdvanceState { get; set; }

        /// <summary>
        /// Delay after tags have been validated before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for tags to be added.
        /// </remarks>
        TimeSpan DelayAfterValidatingTags { get; set; }

        int MinimumNumberOfArgumentsToAdvanceState { get; set; }

        /// <summary>
        /// Delay after the required number of arguments have been presented before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for additional arguments to be added.
        /// </remarks>
        TimeSpan DelayAfterDiscussion { get; set; }
    }
}
