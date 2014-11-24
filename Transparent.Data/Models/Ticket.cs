using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [MaxLength(100)]
        [Required]
        public string Heading { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(10000)]
        [Required]
        public string Body { get; set; }

        [DataType(DataType.Date)]
        [Required]
        // Must be indexed
        public DateTime CreatedDate { get; set; }

        [Required]
        // Must be indexed
        public int Rank { get; set; }

        public virtual ICollection<TicketUserRank> UserRanks { get; set; }
    }
}
