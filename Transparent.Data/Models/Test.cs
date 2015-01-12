using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public class Test : Ticket
    {
        public override TicketType TicketType
        {
            get
            {
                return TicketType.Test;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        public override bool MultipleTags
        {
            get
            {
                return false;
            }
        }
    }
}
