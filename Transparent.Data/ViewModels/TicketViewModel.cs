using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.ViewModels
{
    public class TicketViewModel<TModel>: TicketViewModel
        where TModel : Ticket
    {
        public TModel Ticket { get { return (TModel)Model; } }

        public TicketViewModel(): base(GetTicketType())
        {           
        }

        private static TicketType GetTicketType()
        {
            if (typeof(TModel) == typeof(Suggestion))
                return TicketType.Suggestion;
            if (typeof(TModel) == typeof(Question))
                return TicketType.Question;
            if (typeof(TModel) == typeof(Test))
                return TicketType.Test;
            throw new NotSupportedException();
        }
    }

    public class TicketViewModel
    {
        public Ticket Model { get; private set; }

        public TicketViewModel(TicketType ticketType): this(Ticket.Create(ticketType))
        {
        }

        public TicketViewModel(Ticket model)
        {
            this.Model = model;
        }

        public TicketType TicketType 
        {
            get
            {
                return Model.TicketType;
            }
        }

        public string Heading
        {
            get
            {
                return Model.Heading;
            }
            set
            {
                Model.Heading = value;
            }
        }

        public string Body
        {
            get
            {
                return Model.Body;
            }
            set
            {
                Model.Body = value;
            }
        }

        public UserProfile User
        {
            get
            {
                return Model.User;
            }
            set
            {
                Model.User = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return Model.CreatedDate;
            }
            set
            {
                Model.CreatedDate = value;
            }
        }

        public int Id 
        {
            get
            {
                return Model.Id;
            }
            set
            {
                Model.Id = value;
            }
        }

        public int FkUserId
        {
            get
            {
                return Model.FkUserId;
            }
            set
            {
                Model.FkUserId = value;
            }
        }

        public int[] TicketTagIds
        {
            get
            {
                return Model.TicketTags == null ? new int[]{} : Model.TicketTags.Select(tag => tag.FkTagId).ToArray();
            }
            set
            {
                var ticketTags = new List<TicketTag>();                
                foreach (var id in value.Distinct().Where(id => id >= 0))
                {
                    ticketTags.Add(new TicketTag { FkTagId = id, Ticket = Model });
                }
                Model.TicketTags = ticketTags;
            }
        }
    }
}
