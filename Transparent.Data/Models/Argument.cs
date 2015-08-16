using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Part of a discussion on a suggestion or an answer to a question.
    /// </summary>
    [MetadataType(typeof(IArgument))]
    public class Argument: IArgument
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

        public string Body { get; set; }

        /// <summary>
        /// Expertise of the user that created this argument
        /// </summary>
        [Required]
        [Index]
        public int UserWeighting { get; set; }
    }
}
