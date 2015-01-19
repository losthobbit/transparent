using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TestMarking
    {
        [Key, Column(Order = 0)]
        [ForeignKey("TestPoint")]
        [Required]
        public int FkUserPointId { get; set; }
        public UserPoint TestPoint { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [Required]
        public bool Passed { get; set; }
    }
}
