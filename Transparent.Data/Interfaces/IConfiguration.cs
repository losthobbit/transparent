using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IConfiguration
    {
        string CurrentSubGoal { get; set; }

        int PointsRequiredBeforeDeductingPoints { get; set; }
        int PointsToDeductWhenStartingTest { get; set; }

        int PointsRequiredToBeCompetent { get; set; }
    }
}
