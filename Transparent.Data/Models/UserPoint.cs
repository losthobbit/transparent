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
    /// One for each time points were awarded for a particular tag.
    /// </summary>
    public class UserPoint
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [ForeignKey("Tag")]
        public int FkTagId { get; set; }
        public Tag Tag { get; set; }

        [Display(Name = "Points")]
        public int Quantity { get; set; }

        [ForeignKey("TestTaken")]
        public int FkTestId { get; set; }
        public Test TestTaken { get; set; }

        [MaxLength(2000)]
        public string Answer { get; set; }
    }
}
