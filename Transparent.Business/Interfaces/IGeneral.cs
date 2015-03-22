using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Business.ViewModels;

namespace Transparent.Business.Interfaces
{
    public interface IGeneral
    {
        StatsViewModel GetStats();
    }
}
