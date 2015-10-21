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
    }
}
