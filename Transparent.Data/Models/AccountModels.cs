using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using Transparent.Data.Interfaces;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Data.Models
{
    [MetadataType(typeof(IUserProfile))]
    [Table("UserProfile")]
    public class UserProfile: IUserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Name")]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        public string Services { get; set; }

        public virtual ICollection<UserTag> Tags { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel : IValidatableObject
    {
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace(UserName) && String.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult(String.Format("{0} or {1} must be supplied.",
                    this.GetAttributeFrom<DisplayAttribute>("UserName").Name,
                    this.GetAttributeFrom<DisplayAttribute>("Email").Name), 
                    new []{"UserName", "Email"});
        }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
