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
            CurrentSubGoal = "Increase the number of registered users.";

            PointsRequiredBeforeDeductingPoints = 3;
            PointsToDeductWhenStartingTest = 2;
            PointsForPassingATest = 2;
            MarkersRequiredPerTest = 2;
            PointsMarkersGainForAgreeingATestResult = 1;
            PointsMarkersLoseForDisagreeingATestResult = 1;

            PointsRequiredToBeCompetent = 5;
            PointsRequiredToBeAnExpert = 10;

            DiPointsForAcceptedTicket = 2;
            DiPointsForVolunteering = 10;
            DiPointsForFirstBadge = 2;

            MaxPositionToAdvanceState = 2;
            DelayAfterValidatingTags = TimeSpan.FromSeconds(2);
            MinimumNumberOfArgumentsToAdvanceState = 2;
            DelayAfterDiscussion = TimeSpan.FromSeconds(2);
            DelayForVoting = TimeSpan.FromSeconds(2);
            PercentOfVotesRequiredToAccept = 60;
        }

        public string CurrentSubGoal { get; set; }

        public int PointsRequiredBeforeDeductingPoints { get; set; }
        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsForPassingATest { get; set; }
        public int MarkersRequiredPerTest { get; set; }
        public int PointsMarkersGainForAgreeingATestResult { get; set; }
        public int PointsMarkersLoseForDisagreeingATestResult { get; set; }

        public int PointsRequiredToBeCompetent { get; set; }
        public int PointsRequiredToBeAnExpert { get; set; }

        public int DiPointsForAcceptedTicket { get; set; }
        public int DiPointsForVolunteering { get; set; }
        public int DiPointsForFirstBadge { get; set; }

        public int MaxPositionToAdvanceState { get; set; }
        public TimeSpan DelayAfterValidatingTags { get; set; }
        public int MinimumNumberOfArgumentsToAdvanceState { get; set; }
        public TimeSpan DelayAfterDiscussion { get; set; }
        public TimeSpan DelayForVoting { get; set; }
        public int PercentOfVotesRequiredToAccept { get; set; }
    }
}
