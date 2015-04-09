using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TicketHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Ticket")]
        [Required]
        public int FkTicketId { get; set; }
        public Ticket Ticket { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public virtual UserProfile User { get; set; }

        [Required]
        public TicketState State { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }
    }
}
