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

        public string CurrentSubGoal { get; set; }

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

        public int PointsRequiredToBeCompetent { get; set; }
        public int PointsRequiredToBeAnExpert { get; set; }

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
            CurrentSubGoal = configuration.GetValue("CurrentSubGoal");

            PointsRequiredBeforeDeductingPoints = int.Parse(configuration.GetValue("PointsRequiredBeforeDeductingPoints"));
            PointsToDeductWhenStartingTest = int.Parse(configuration.GetValue("PointsToDeductWhenStartingTest"));
            PointsForPassingATest = int.Parse(configuration.GetValue("PointsForPassingATest"));
            MarkersRequiredPerTest = int.Parse(configuration.GetValue("MarkersRequiredPerTest"));
            PointsMarkersGainForAgreeingATestResult = int.Parse(configuration.GetValue("PointsMarkersGainForAgreeingATestResult"));
            PointsMarkersLoseForDisagreeingATestResult = int.Parse(configuration.GetValue("PointsMarkersLoseForDisagreeingATestResult"));

            PointsRequiredToBeCompetent = int.Parse(configuration.GetValue("PointsRequiredToBeCompetent"));
            PointsRequiredToBeAnExpert = int.Parse(configuration.GetValue("PointsRequiredToBeAnExpert"));

            MaxPositionToAdvanceState = int.Parse(configuration.GetValue("MaxPositionToAdvanceState"));
            DelayAfterValidatingTags = TimeSpan.Parse(configuration.GetValue("DelayAfterValidatingTags"));
            MinimumNumberOfArgumentsToAdvanceState = int.Parse(configuration.GetValue("MinimumNumberOfArgumentsToAdvanceState"));
            DelayAfterDiscussion = TimeSpan.Parse(configuration.GetValue("DelayAfterDiscussion"));
            DelayForVoting = TimeSpan.Parse(configuration.GetValue("DelayForVoting"));
            PercentOfVotesRequiredToAccept = int.Parse(configuration.GetValue("PercentOfVotesRequiredToAccept"));
        }
    }
}
