using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Interfaces
{
    public interface IArgument
    {
        [DataType(DataType.MultilineText)]
        [StringLength(10000, ErrorMessage = "Only 10,000 characters allowed.")]
        [Required(ErrorMessage = "Cannot be blank.")]
        string Body { get; set; }
    }
}
