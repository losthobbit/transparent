using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Extensions
{
    public static class ModelViewExtensions
    {
        public static IEnumerable<TicketTypeInfo> TicketTypeInfo { get; set; }
        
        static ModelViewExtensions()
        {
            TicketTypeInfo = CreateTicketTypeInfo();
        }

        private static IEnumerable<TicketTypeInfo> CreateTicketTypeInfo()
        {
            return
                new[] { (TicketType?)null }.Union(((TicketType[])Enum.GetValues(typeof(TicketType))).Select(tt => (TicketType?)tt))
                .Select(tt => new TicketTypeInfo { Type = tt, DisplayName = tt.DisplayName(), PluralDisplayName = tt.PluralDisplayName() });
        }

        public static string GetClass(this TicketType? ticketType)
        {
            return ticketType == null ? string.Empty : GetClass(ticketType.Value);
        }
        
        public static string GetClass(this TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return "question";
                case TicketType.Suggestion: return "suggestion";
                case TicketType.Test: return "test";
            }
            throw new NotSupportedException("Unknown ticket type");
        }

        public static string GetChar(this TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Question: return "Q";
                case TicketType.Suggestion: return "S";
                case TicketType.Test: return "T";
            }
            throw new NotSupportedException("Unknown ticket type");
        }
    }

    public class TicketTypeInfo
    {
        public TicketType? Type { get; set; }
        public string DisplayName { get; set; }
        public string PluralDisplayName { get; set; }
    }
}