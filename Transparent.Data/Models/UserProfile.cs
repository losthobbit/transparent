using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Data.Models
{
    [MetadataType(typeof(IUserProfile))]
    [Table("UserProfile")]
    public class UserProfile : IUserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Username")]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        public string Services { get; set; }

        public int Badges { get; set; }

        public virtual ICollection<UserTag> Tags { get; set; }

        public bool HasBadge(Badge badge)
        {
            return ((Badge)Badges & badge) == badge;
        }

        public void SetBadge(Badge badge)
        {
            Badges = ((int)badge | Badges);
        }

        [Index]
        public DateTime? LastActionDate { get; set; }

        [Index(IsUnique=true)]
        [StringLength(32)]
        public string FacebookId { get; set; }
    }

}
