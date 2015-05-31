using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    public class UserTagViewModel: TagViewModel
    {
        public int TotalPoints { get; set; }
        public KnowledgeLevel KnowledgeLevel { get; set; }
    }
}
