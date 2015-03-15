using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models.Interfaces;
using Transparent.Data.Validation;

namespace Transparent.Data.Models
{
    [MetadataType(typeof(IBaseTicket))]
    public abstract class BaseTicket: IBaseTicket, ISupportsMultipleTags
    {
        protected BaseTicket()
        {
        }

        protected BaseTicket(int id, int rank)
            : this()
        {
            Id = id;
            Rank = rank;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Index]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Index]
        public int Rank { get; set; }

        [Display(Name="Title")]
        public string Heading { get; set; }

        public string Body { get; set; }

        [Required]
        [Index]
        public TicketState State { get; set; }

        /// <summary>
        /// Date that information about the ticket was updated.
        /// </summary>
        /// <remarks>
        /// This does not necessarily mean that the change occured in this class.
        /// It could, for example mean that a tag has been verified.
        /// </remarks>
        [DataType(DataType.Date)]
        [Required]
        [Index]
        public DateTime ModifiedDate { get; set; }

        // Specified in fluent API instead: 
        [NotMapped()]
        public virtual TicketType TicketType { get; protected set; }

        // No validation attributes, because this gets validated in the view model.
        [Display(Name = "Tags")]
        public virtual ICollection<TicketTag> TicketTags { get; set; }

        [NotMapped]
        public virtual bool MultipleTags { get { return true; } }

        [NotMapped]
        public virtual string TextForCreated { get { return "Created"; } }
    }
}
