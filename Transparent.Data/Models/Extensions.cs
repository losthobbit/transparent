using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Models
{
    public static class Extensions
    {
        public static T GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).First();
        }
    }
}
