using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Services
{
    public class Configuration : IConfiguration
    {
        #region General

        public string CurrentSubgoal { get; set; }

        #endregion General

        #region Points

        #region Tests

        public int PointsRequiredBeforeDeductingPoints { get; set; }
        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsForPassingATest { get; set; }
        public int MarkersRequiredPerTest { get; set; }
        public int PointsMarkersGainForAgreeingATestResult { get; set; }
        public int PointsMarkersLoseForDisagreeingATestResult { get; set; }

        #endregion Tests

        /// <summary>
        /// Percent of highest score in a tag that will guarantee competent level.
        /// </summary>
        public int CompetentPercentOfHighestScore { get; set; }
        /// <summary>
        /// Percent of people with who will be considered competent for a tag.
        /// </summary>
        public int MinPercentCompetents { get; set; }
        /// <summary>
        /// Minimum number of users who will be considered competent for a tag.
        /// </summary>
        public int MinCompetents { get; set; }

        /// <summary>
        /// Percent of highest score in a tag that will guarantee expert level.
        /// </summary>
        public int ExpertPercentOfHighestScore { get; set; }
        /// <summary>
        /// Percent of users who will be considered experts for a tag.
        /// </summary>
        public int MinPercentExperts { get; set; }
        /// <summary>
        /// Minimum number of users who will be considered experts for a tag.
        /// </summary>
        public int MinExperts { get; set; }

        public int DiPointsForAcceptedTicket { get; set; }
        public int DiPointsForVolunteering { get; set; }
        public int DiPointsForFirstBadge { get; set; }

        #endregion Points

        #region Progressing tickets

        /// <summary>
        /// The maximum position (based on highest ranked within a state) that a ticket can
        /// be in before being allowed to advance to the next state via a timed event.
        /// </summary>
        /// <remarks>
        /// This prevents low ranked tickets from progressing faster than high ranked tickets.
        /// </remarks>
        public int MaxPositionToAdvanceState { get; set; }

        /// <summary>
        /// Delay after tags have been validated before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for tags to be added.
        /// </remarks>
        public TimeSpan DelayAfterValidatingTags { get; set; }

        public int MinimumNumberOfArgumentsToAdvanceState { get; set; }
        public int MinimumNumberOfAnswersToAdvanceState { get; set; }

        /// <summary>
        /// Delay after the required number of arguments have been presented before moving a ticket to the next state.
        /// </summary>
        /// <remarks>
        /// This allows time for additional arguments to be added.
        /// </remarks>
        public TimeSpan DelayAfterDiscussion { get; set; }

        /// <summary>
        /// Time waiting for votes before moving a ticket to the next state
        /// </summary>
        /// <remarks>
        /// This allows time for users to vote.
        /// </remarks>
        public TimeSpan DelayForVoting { get; set; }

        /// <summary>
        /// The percentage of votes required in order to accept the ticket
        /// </summary>
        public int PercentOfVotesRequiredToAccept { get; set; }

        #endregion Progressing tickets

        public Configuration(Common.Interfaces.IConfiguration configuration)
        {
            CurrentSubgoal = configuration.GetValue("CurrentSubGoal");

            PointsRequiredBeforeDeductingPoints = int.Parse(configuration.GetValue("PointsRequiredBeforeDeductingPoints"));
            PointsToDeductWhenStartingTest = int.Parse(configuration.GetValue("PointsToDeductWhenStartingTest"));
            PointsForPassingATest = int.Parse(configuration.GetValue("PointsForPassingATest"));
            MarkersRequiredPerTest = int.Parse(configuration.GetValue("MarkersRequiredPerTest"));
            PointsMarkersGainForAgreeingATestResult = int.Parse(configuration.GetValue("PointsMarkersGainForAgreeingATestResult"));
            PointsMarkersLoseForDisagreeingATestResult = int.Parse(configuration.GetValue("PointsMarkersLoseForDisagreeingATestResult"));

            CompetentPercentOfHighestScore = int.Parse(configuration.GetValue("CompetentPercentOfHighestScore"));
            MinPercentCompetents = int.Parse(configuration.GetValue("MinPercentCompetents"));
            MinCompetents = int.Parse(configuration.GetValue("MinCompetents"));

            ExpertPercentOfHighestScore = int.Parse(configuration.GetValue("ExpertPercentOfHighestScore"));
            MinPercentExperts = int.Parse(configuration.GetValue("MinPercentExperts"));
            MinExperts = int.Parse(configuration.GetValue("MinExperts"));

            DiPointsForAcceptedTicket = int.Parse(configuration.GetValue("DiPointsForAcceptedTicket"));
            DiPointsForVolunteering = int.Parse(configuration.GetValue("DiPointsForVolunteering"));
            DiPointsForFirstBadge = int.Parse(configuration.GetValue("DiPointsForFirstBadge"));

            MaxPositionToAdvanceState = int.Parse(configuration.GetValue("MaxPositionToAdvanceState"));
            DelayAfterValidatingTags = TimeSpan.Parse(configuration.GetValue("DelayAfterValidatingTags"));
            MinimumNumberOfArgumentsToAdvanceState = int.Parse(configuration.GetValue("MinimumNumberOfArgumentsToAdvanceState"));
            MinimumNumberOfAnswersToAdvanceState = int.Parse(configuration.GetValue("MinimumNumberOfAnswersToAdvanceState"));
            DelayAfterDiscussion = TimeSpan.Parse(configuration.GetValue("DelayAfterDiscussion"));
            DelayForVoting = TimeSpan.Parse(configuration.GetValue("DelayForVoting"));
            PercentOfVotesRequiredToAccept = int.Parse(configuration.GetValue("PercentOfVotesRequiredToAccept"));
        }
    }
}
