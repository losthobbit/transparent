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
        }

        public string CurrentSubGoal { get; set; }

        public int PointsRequiredBeforeDeductingPoints { get; set; }
        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsForPassingATest { get; set; }
        public int MarkersRequiredPerTest { get; set; }
        public int PointsMarkersGainForAgreeingATestResult { get; set; }
        public int PointsMarkersLoseForDisagreeingATestResult { get; set; }

        public int PointsRequiredToBeCompetent { get; set; }
    }
}
