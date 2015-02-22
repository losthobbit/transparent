using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    /// <summary>
    /// Allows a user to visitor to subscribe to news without being registered.
    /// </summary>
    /// <remarks>
    /// I may consider removing this later and encourage people to register in order to receive news (or auto-register them and send them a password).
    /// </remarks>
    public class Subscription
    {
        [Key]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [StringLength(100)]
        [Required(ErrorMessage="Email address is required")]
        public string Email { get; set; }
    }
}
