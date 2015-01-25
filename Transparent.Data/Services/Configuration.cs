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
        public int PointsToDeductWhenStartingTest { get; set; }
        public int PointsRequiredToMarkTest { get; set; }

        public Configuration(Common.Interfaces.IConfiguration configuration)
        {
            PointsToDeductWhenStartingTest = int.Parse(configuration.GetValue("PointsToDeductWhenStartingTest"));
            PointsRequiredToMarkTest = int.Parse(configuration.GetValue("PointsRequiredToMarkTest"));
        }
    }
}
