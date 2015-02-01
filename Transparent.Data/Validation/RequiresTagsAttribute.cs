using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Transparent.Data.Models;

namespace Transparent.Data.Validation
{
    /// <summary>
    /// Specifies that tags are required.
    /// </summary>
    /// <remarks>
    /// Class must implement ISupportsMultipleTags
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiresTagsAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var multipleTags = ((ISupportsMultipleTags)validationContext.ObjectInstance).MultipleTags;
            return ((IEnumerable)value).Any() ? ValidationResult.Success :
                new ValidationResult(multipleTags ? "Tags are required." : "A tag is required");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ValidationType = "requiresTag";
            rule.ErrorMessage = "A tag is required";
            return new List<ModelClientValidationRule> { rule };
        }
    }
}
