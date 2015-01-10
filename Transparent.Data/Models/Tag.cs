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
        [MaxLength(60)]
        [Display(Name = "Tag")]
        // Must be indexed
        public string Name { get; set; }

        [Required]
        [MaxLength(10000)]
        [Display(Name = "Description")]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        public ICollection<Tag> Parents { get; set; }
        public ICollection<Tag> Children { get; set; }
    }
}
