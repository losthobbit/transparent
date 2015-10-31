using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IKnowledgeLevelWeightings
    {
        /// <summary>
        /// Weighting to apply for beginners
        /// </summary>
        int BeginnerWeighting { get; set; }

        /// <summary>
        /// Weighting to apply for competent users
        /// </summary>
        int CompetentWeighting { get; set; }

        /// <summary>
        /// Weighting to apply for experts
        /// </summary>
        int ExpertWeighting { get; set; }
    }
}
