using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models.Interfaces
{
    public interface ITestAnswer
    {
        [DataType(DataType.MultilineText)]
        [MaxLength(2000)]
        string Answer { get; set; }
    }
}
