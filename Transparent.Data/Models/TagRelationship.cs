using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TagRelationship
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Parent")]
        [Required]
        public int FkParentId { get; set; }
        public virtual Tag Parent { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Child")]
        [Required]
        public int FkChildId { get; set; }
        public virtual Tag Child { get; set; }
    }
}
