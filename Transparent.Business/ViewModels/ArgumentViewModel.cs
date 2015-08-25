using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Transparent.Data.Interfaces;

namespace Transparent.Business.ViewModels
{
    /// <summary>
    /// Part of a discussion on a suggestion or an answer to a question.
    /// </summary>
    [MetadataType(typeof(IArgument))]
    public class ArgumentViewModel: IArgument
    {
        public string Caption { get; set; }
        public int FkTicketId { get; set; }
        public string Body { get; set; }
        public int FkUserId { get; set; }
        public UserSummaryViewModel User { get; set; }
        public int UserWeighting { get; set; }

        /// <summary>
        /// Message to tell user that the argument has been saved.
        /// </summary>
        public string Message { get; set; }
    }
}