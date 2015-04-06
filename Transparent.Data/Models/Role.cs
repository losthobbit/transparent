using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    [Table("webpages_Roles")]
    public class Role
    {
        public Role()
        {
            UsersInRoles = new List<UserInRole>();
        }

        [Key]
        public int RoleId { get; set; }
        [StringLength(256)]
        public string RoleName { get; set; }

        [ForeignKey("RoleId")]
        public ICollection<UserInRole> UsersInRoles { get; set; }
    }
}
