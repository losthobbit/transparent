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

        #endregion Points

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
        }
    }
}
