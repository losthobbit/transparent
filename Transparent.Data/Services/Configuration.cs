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

        #endregion Tests

        public int PointsRequiredToBeCompetent { get; set; }

        #endregion Points

        public Configuration(Common.Interfaces.IConfiguration configuration)
        {
            PointsRequiredBeforeDeductingPoints = int.Parse(configuration.GetValue("PointsRequiredBeforeDeductingPoints"));
            PointsToDeductWhenStartingTest = int.Parse(configuration.GetValue("PointsToDeductWhenStartingTest"));
            PointsRequiredToBeCompetent = int.Parse(configuration.GetValue("PointsRequiredToBeCompetent"));
            CurrentSubGoal = configuration.GetValue("CurrentSubGoal");
        }
    }
}
