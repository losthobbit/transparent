using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IConfiguration
    {
        string CurrentSubgoal { get; set; }

        /// <summary>
        /// After this period of inactivity, a user is considered inactive.
        /// </summary>
        TimeSpan UserActiveTime { get; set; }

        int PointsRequiredBeforeDeductingPoints { get; set; }
        int PointsToDeductWhenStartingTest { get; set; }
        int PointsForPassingATest { get; set; }
        int MarkersRequiredPerTest { get; set; }
        int PointsMarkersGainForAgreeingATestResult { get; set; }
        int PointsMarkersLoseForDisagreeingATestResult { get; set; }

        /// <summary>
        /// Percent of highest score in a tag that will guarantee competent level.
        /// </summary>
        int CompetentPercentOfHighestScore { get; set; }
        /// <summary>
        /// Minimum percent of users who will be considered competent for a tag.
        /// </summary>
        int MinPercentCompetents { get; set; }
        /// <summary>
        /// Minimum number of users who will be considered competent for a tag.
        /// </summary>
        int MinCompetents { get; set; }

        /// <summary>
        /// Percent of highest score in a tag that will guarantee expert level.
        /// </summary>
        int ExpertPercentOfHighestScore { get; set; }
        /// <summary>
        /// Minimum percent of users who will be considered experts for a tag.
        /// </summary>
        int MinPercentExperts { get; set; }
        /// <summary>
        /// Minimum number of users who will be considered experts for a tag.
        /// </summary>
        int MinExperts { get; set; }

        int DiPointsForAcceptedTicket { get; set; }
        int DiPointsForVolunteering { get; set; }
        int DiPointsForFirstBadge { get; set; }

        /// <summary>
        /// Weighting to apply to arguments / answers of beginners
        /// </summary>
        int BeginnerWeighting { get; set; }

        /// <summary>
        /// Weighting to apply to arguments / answers of competent users
        /// </summary>
        int CompetentWeighting { get; set; }

        /// <summary>
        /// Weighting to apply to arguments / answers of experts
        /// </summary>
        int ExpertWeighting { get; set; }

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
        int MinimumNumberOfAnswersToAdvanceState { get; set; }

        /// <summary>
        /// Delay after the required number of arguments have been presented before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for additional arguments to be added.
        /// </remarks>
        TimeSpan DelayAfterDiscussion { get; set; }

        /// <summary>
        /// Time waiting for votes before moving a ticket to the next state
        /// </summary>
        /// <remarks>
        /// This allows time for users to vote.
        /// </remarks>
        TimeSpan DelayForVoting { get; set; }

        /// <summary>
        /// The percentage of votes required in order to accept the ticket
        /// </summary>
        int PercentOfVotesRequiredToAccept { get; set; }
    }
}
