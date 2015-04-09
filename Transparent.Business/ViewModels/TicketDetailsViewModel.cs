using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Common;
using Transparent.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Transparent.Business.ViewModels
{
    /// <summary>
    /// For viewing ticket details.
    /// </summary>
    [MetadataType(typeof(IBaseTicket))]
    public class TicketDetailsViewModel: IBaseTicket
    {
        public TicketDetailsViewModel()
        {

        }

        public IEnumerable<TicketTagViewModel> TagInfo { get; set; }
        public ICollection<ArgumentViewModel> Arguments { get; set; }

        public string TextForCreated { get; set; }
        public string TextForArgument { get; set; }

        public bool MultipleTags { get; set; }

        public int Rank { get; set; }
        public Stance UserRank { get; set; }
        public Stance NewRank { get; set; }

        public VoteViewModel Vote { get; set; }

        public int Id { get; set; }
        public string Heading { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public TicketType TicketType { get; set; }
        public ICollection<TicketTag> TicketTags { get; set; }

        public TicketState State { get; set; }

        public string AssignedName { get; set; }
    }
}
