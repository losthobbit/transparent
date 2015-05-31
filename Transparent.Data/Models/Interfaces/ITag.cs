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
    public interface ITag
    {
        [Required]
        [StringLength(60)]
        [Display(Name = "Tag")]
        string Name { get; set; }

        [Required]
        [StringLength(10000)]
        [Display(Name = "Description")]
        [DataType(DataType.Html)]
        string Description { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered competent.
        /// </summary>
        int CompetentPoints { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered an expert.
        /// </summary>
        int ExpertPoints { get; set; }
    }
}
