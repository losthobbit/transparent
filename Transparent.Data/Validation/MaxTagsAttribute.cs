using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Validation
{
    /// <summary>
    /// Determines the maximum number of tags allowed.
    /// </summary>
    /// <remarks>
    /// Class must implement ISupportsMultipleTags
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxTagsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ((ISupportsMultipleTags)validationContext.ObjectInstance).MultipleTags || ((IEnumerable)value).Count() <= 1 ?
                ValidationResult.Success :
                new ValidationResult("Only one tag allowed.");
        }
    }
}
