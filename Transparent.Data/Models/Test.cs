﻿using System;
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

        [NotMapped()]
        public int TagId
        {
            get
            {
                if (TicketTags == null)
                    return -1;
                // Will thrown an exception if there are no tags.  This is fine when the tags have not yet been validated.
                return TicketTags.Single().FkTagId;
            }
            set
            {
                if(TicketTags == null)
                    TicketTags = new List<TicketTag>();
                TicketTags.Add( new TicketTag { FkTagId = value } );
            }
        }
    }
}
