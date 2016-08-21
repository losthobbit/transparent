﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;

namespace Transparent.Business.ViewModels
{
    /// <summary>
    /// Used to display tags in partial view so that tags can be verified.
    /// </summary>
    public class TicketTagsViewModel
    {
        public bool EnableTagButton { get; set; }
        public bool MultipleTags { get; set; }
        public int TicketId { get; set; }
        public IEnumerable<TicketTagViewModel> TagInfo { get; set; }

        // Actions

        public int? ForId { get; set; }
        public int? AgainstId { get; set; }
        public int? NeutralId { get; set; }

        public TicketTagsViewModel()
        {

        }

        public TicketTagsViewModel(BaseTicket ticket, bool enableTagButton,
            IThresholds thresholds, IEnumerable<TicketTagViewModel> tagInfo = null)
        {
            EnableTagButton = enableTagButton;
            MultipleTags = ticket.MultipleTags;
            TicketId = ticket.Id;

            TagInfo = TicketTagViewModel.CreateList(ticket, thresholds, tagInfo);
        }
    }
}
