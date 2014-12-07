using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class UserTag
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Tag")]
        [Required]
        public int FkTagId { get; set; }
        public virtual Tag Tag { get; set; }

        [Display(Name = "Points")]
        public int TotalPoints { get; set; }
    }
}
