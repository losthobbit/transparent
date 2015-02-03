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
            PointsToDeductWhenStartingTest = 2;
            PointsRequiredToBeCompetent = 5;
            CurrentSubGoal = "Increase the number of registered users.";
        }

        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsRequiredToBeCompetent { get; set; }
        public string CurrentSubGoal { get; set; }
    }
}
