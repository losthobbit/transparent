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
    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Username")]
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
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class LoginModel : IValidatableObject
    {
        [Display(Name = "Username")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string Action { get; set; }

        public string FacebookToken { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace(UserName) && String.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult(String.Format("{0} or {1} must be supplied.",
                    this.GetAttributeFrom<DisplayAttribute>("UserName").Name,
                    this.GetAttributeFrom<DisplayAttribute>("Email").Name), 
                    new []{"UserName", "Email"});

            if (Action == "LogIn" && String.IsNullOrWhiteSpace(Password))
                yield return new ValidationResult(String.Format("{0} must be supplied.",
                    this.GetAttributeFrom<DisplayAttribute>("Password").Name),
                    new []{"Password"});
        }
    }

    public class RegisterModel : IValidatableObject
    {
        private const int MinPasswordLength = 8;

        [Required]
        [Display(Name = "Username")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }

        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Determines whether or not to get the user's Facebook token to register.
        /// </summary>
        public bool? GetFacebookToken { get; set; }

        public string FacebookToken { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FacebookToken))
            {
                if (String.IsNullOrWhiteSpace(Password))
                {
                    yield return new ValidationResult(String.Format("{0} must be supplied.",
                        this.GetAttributeFrom<DisplayAttribute>("Password").Name),
                        new[] { "Password" });
                }
                if (Password.Length < MinPasswordLength)
                {
                    yield return new ValidationResult(String.Format("The {0} must be at least {1} characters long.",
                        this.GetAttributeFrom<DisplayAttribute>("Password").Name, MinPasswordLength),
                        new[] { "Password" });
                }
            }
        }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
