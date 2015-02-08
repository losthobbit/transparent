using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models.Interfaces
{
    /// <summary>
    /// Contains reusable data annotations
    /// </summary>
    public interface IBaseTicket
    {
        [StringLength(100, ErrorMessage="The heading may not contain more than 100 characters.")]
        [Required]
        string Heading { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(10000, ErrorMessage="The body may not contain more than 10,000 characters.")]
        [Required]
        string Body { get; set; }
    }
}
