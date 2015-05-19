using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Contains a hint and a URL for more information.
    /// </summary>
    public class Hint
    {
        /// <summary>
        /// Creates an instance of a Hint.
        /// </summary>
        /// <param name="details">Description of the hint for the user.</param>
        /// <param name="url">Relative URL for more information.</param>
        /// <param name="showState">Whether the state should be displayed to the user.</param>
        public Hint(string details = null, string url = null, bool showState = true)
        {
            this.Details = details;
            this.Url = url;
            this.ShowState = showState;
        }

        public string Details { get; set; }

        /// <summary>
        /// Relative URL for more information.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Whether the state should be displayed to the user.
        /// </summary>
        public bool ShowState { get; set; }
    }
}
