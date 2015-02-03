using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class TicketTag
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Ticket")]
        [Required]
        public int FkTicketId { get; set; }
        public Ticket Ticket { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Tag")]
        [Required]
        public int FkTagId { get; set; }
        public virtual Tag Tag { get; set; }

        /// <summary>
        /// If null, assumed to be the Ticket's user ID
        /// </summary>
        [ForeignKey("CreatedBy")]
        public int? FkCreatedById { get; set; }
        public UserProfile CreatedBy { get; set; }

        public bool Verified { get; set; }
    }
}
