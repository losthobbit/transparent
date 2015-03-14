using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Interfaces
{
    public interface ITechnicalConfiguration
    {
        TimeSpan MinEventInterval { get; set; }
    }
}
