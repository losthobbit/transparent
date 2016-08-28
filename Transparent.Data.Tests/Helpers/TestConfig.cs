using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Tests.Helpers
{
    public class TestConfig: IConfiguration
    {
        public TestConfig()
        {
            CurrentSubgoal = "Increase the number of registered users.";
            BaseSiteUrl = "https://democraticintelligence.org";

            UserActiveTime = new TimeSpan(10, 0, 0, 0);

            PointsRequiredBeforeDeductingPoints = 3;
            PointsToDeductWhenStartingTest = 2;
            PointsForPassingATest = 2;
            MarkersRequiredPerTest = 2;
            PointsMarkersGainForAgreeingATestResult = 1;
            PointsMarkersLoseForDisagreeingATestResult = 1;

            CompetentPercentOfHighestScore = 50;
            MinPercentCompetents = 50;
            MinCompetents = 2;

            ExpertPercentOfHighestScore = 75;
            MinPercentExperts = 20;
            MinExperts = 2;

            DiPointsForAcceptedTicket = 2;
            DiPointsForVolunteering = 10;
            DiPointsForFirstBadge = 2;

            BeginnerWeighting = 1;
            CompetentWeighting = 2;
            ExpertWeighting = 4;

            FullAcceptanceThreshold = 10;
            NotAcceptedThreshold = 3;

            MaxPositionToAdvanceState = 2;
            MinimumNumberOfArgumentsToAdvanceState = 2;
            MinimumNumberOfAnswersToAdvanceState = 1;
            MaximumNumberOfTicketsInVotingState = 20;

            DelayAfterDiscussion = TimeSpan.FromSeconds(2);
            DelayForVoting = TimeSpan.FromSeconds(2);
            PercentOfVotesRequiredToAccept = 60;
        }

        public string CurrentSubgoal { get; set; }

        public string BaseSiteUrl { get; set; }

        /// <summary>
        /// After this period of inactivity, a user is considered inactive.
        /// </summary>
        public TimeSpan UserActiveTime { get; set; }

        public int PointsRequiredBeforeDeductingPoints { get; set; }
        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsForPassingATest { get; set; }
        public int MarkersRequiredPerTest { get; set; }
        public int PointsMarkersGainForAgreeingATestResult { get; set; }
        public int PointsMarkersLoseForDisagreeingATestResult { get; set; }

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

        /// <summary>
        /// Weighting to apply to arguments / answers of beginners
        /// </summary>
        public int BeginnerWeighting { get; set; }

        /// <summary>
        /// Weighting to apply to arguments / answers of competent users
        /// </summary>
        public int CompetentWeighting { get; set; }

        /// <summary>
        /// Weighting to apply to arguments / answers of experts
        /// </summary>
        public int ExpertWeighting { get; set; }

        /// <summary>
        /// The number of points on a tag to indicate that it is 100% accepted.
        /// </summary>
        public int FullAcceptanceThreshold { get; set; }

        /// <summary>
        /// The minimum number of points on a tag to make the tag count.
        /// </summary>
        public int NotAcceptedThreshold { get; set; }

        public int MaxPositionToAdvanceState { get; set; }
        public int MinimumNumberOfArgumentsToAdvanceState { get; set; }
        public int MinimumNumberOfAnswersToAdvanceState { get; set; }
        public TimeSpan DelayAfterDiscussion { get; set; }
        public int MaximumNumberOfTicketsInVotingState { get; set; }
        public TimeSpan DelayForVoting { get; set; }
        public int PercentOfVotesRequiredToAccept { get; set; }
    }
}
