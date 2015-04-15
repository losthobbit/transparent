using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Also known as a tag or category
    /// </summary>
    public class Tag
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        [Display(Name = "Tag")]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        [Required]
        [StringLength(10000)]
        [Display(Name = "Description")]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered competent.
        /// </summary>
        public int CompetentPoints { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered an expert.
        /// </summary>
        public int ExpertPoints { get; set; }

        public ICollection<Tag> Parents { get; set; }
        public ICollection<Tag> Children { get; set; }
    }
}
