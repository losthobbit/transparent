using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public abstract class BaseVoteViewModel: IVoteViewModel
    {
        public abstract int Id { get; set; }
        public Stance UserVote { get; set; }
        public bool UserMayVote { get; set; }

        /// <summary>
        /// The total number of points, used to determine the percentage acceptance.
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// The number of points on a tag to indicate that it is 100% accepted.
        /// </summary>
        public int FullAcceptanceThreshold { get; set; }

        /// <summary>
        /// The minimum number of points on a tag to make the tag count.
        /// </summary>
        public int NotAcceptedThreshold { get; set; }
    }
}
