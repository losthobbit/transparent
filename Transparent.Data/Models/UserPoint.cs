using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class UserPoint
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [ForeignKey("Tag")]
        public int FkTag { get; set; }
        public Tag Tag { get; set; }

        [Display(Name = "Points")]
        public int Quantity { get; set; }
    }
}
