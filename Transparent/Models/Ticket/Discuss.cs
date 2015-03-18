﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Transparent.Data.Interfaces;

namespace Transparent.Models.Ticket
{
    /// <summary>
    /// Part of a discussion on a suggestion or an answer to a question.
    /// </summary>
    [MetadataType(typeof(IArgument))]
    public class Discuss: IArgument
    {
        public string Caption { get; set; }
        public int FkTicketId { get; set; }
        public string Body { get; set; }
    }
}