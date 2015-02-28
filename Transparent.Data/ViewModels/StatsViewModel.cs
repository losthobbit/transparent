using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.ViewModels
{
    public class StatsViewModel
    {
        [Display(Name = "Registered users")]
        public int RegisteredUsers { get; set; }

        [Display(Name = "Suggestions created")]
        public int SuggestionsCreated { get; set; }

        [Display(Name = "Suggestions implemented")]
        public int SuggestionsImplemented { get; set; }
    }
}
