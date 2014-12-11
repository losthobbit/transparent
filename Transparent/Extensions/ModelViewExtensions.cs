using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Extensions
{
    public static class ModelViewExtensions
    {
        public static string GetClass(this TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return "question";
                case TicketType.Suggestion: return "suggestion";
            }
            throw new NotSupportedException("Unknown ticket type");
        }

        public static string GetChar(this TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return "Q";
                case TicketType.Suggestion: return "S";
            }
            throw new NotSupportedException("Unknown ticket type");
        }
    }
}