using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TemporaryPassword
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public virtual UserProfile User { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime ExpiryDate { get; set; }

        [StringLength(128)]
        [Required]
        public string Hash { get; set; }
    }
}
