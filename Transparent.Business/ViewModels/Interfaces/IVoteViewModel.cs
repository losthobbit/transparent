using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels.Interfaces
{
    public interface IVoteViewModel: IThresholds
    {
        /// <summary>
        /// NOT SURE EXACTLY WHAT THIS ID IS FOR AND IF IT'S NECESSARY... WILL FIGURE OUT SOON, I SUPPOSE
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// How the current user voted.
        /// </summary>
        Stance UserVote { get; set; }

        /// <summary>
        /// Whether the current user may vote.
        /// </summary>
        bool UserMayVote { get; set; }

        /// <summary>
        /// The total number of points, used to determine the percentage acceptance.
        /// </summary>
        int TotalPoints { get; set; }
    }
}
