using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Ticket : BaseTicket
    {
        public Ticket()
        {

        }

        public Ticket(int id, int rank):base(id, rank)
        {
        }

        /// <summary>
        /// The user that created the ticket
        /// </summary>
        [ForeignKey("User")]
        [Required]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [DataType(DataType.Date)]
        [Required]
        // Must be indexed
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<TicketUserRank> UserRanks { get; set; }

        [Display(Name = "Tags")]
        public virtual ICollection<TicketTag> TicketTags { get; set; }

        public static Ticket Create(TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return new Question();
                case TicketType.Suggestion: return new Suggestion();
                case TicketType.Test: return new Test();
            }
            throw new NotSupportedException("Unknown ticket type");
        }
    }
}
