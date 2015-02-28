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
            PointsRequiredBeforeDeductingPoints = 3;
            PointsToDeductWhenStartingTest = 2;
            PointsRequiredToBeCompetent = 5;
            CurrentSubGoal = "Increase the number of registered users.";
        }

        public string CurrentSubGoal { get; set; }

        public int PointsRequiredBeforeDeductingPoints { get; set; }
        public int PointsToDeductWhenStartingTest { get; set; }

        public int PointsRequiredToBeCompetent { get; set; }
    }
}
