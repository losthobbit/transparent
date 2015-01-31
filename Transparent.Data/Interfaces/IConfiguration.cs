using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IConfiguration
    {
        int PointsToDeductWhenStartingTest { get; set; }
        int PointsRequiredToMarkTest { get; set; }
        string CurrentSubGoal { get; set; }
    }
}
