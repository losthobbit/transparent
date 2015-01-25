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
            PointsRequiredToMarkTest = 5;
        }

        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsRequiredToMarkTest { get; set; }
    }
}
