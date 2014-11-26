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
    /// Shows whether a specific ticket has been ranked up or down by a specific user.
    /// </summary>
    public class TicketUserRank
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Ticket")]
        [Required]
        public int FkTicketId { get; set; }
        public Ticket Ticket { get; set; }
        
        [Key, Column(Order = 1)]
        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public virtual UserProfile User { get; set; }

        /// <summary>
        /// True is up, False is down.  Null is not ranked.
        /// </summary>
        [Required]
        public bool Up { get; set; }
    }
}
