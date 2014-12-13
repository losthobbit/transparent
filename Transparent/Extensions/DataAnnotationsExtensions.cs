using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Transparent.Extensions
{
    public static class DataAnnotationsExtensions
    {
        /// <summary>
        /// Returns text describing the object.
        /// </summary>
        /// <remarks>
        /// TODO: Consider using reflection.
        /// (see http://stackoverflow.com/questions/5015830/get-the-value-of-displayname-attribute)
        /// </remarks>
        /// <param name="obj">Object to get the display name for - typically an enum value.</param>
        /// <returns>The display name of the object.</returns>
        public static string DisplayName(this object obj)
        {
            if (obj == null)
                return "None";
            return obj.ToString();
        }

        /// <summary>
        /// Returns text describing the plural of the object.
        /// </summary>
        /// <remarks>
        /// TODO: Consider using reflection.
        /// (see http://stackoverflow.com/questions/5015830/get-the-value-of-displayname-attribute)
        /// </remarks>
        /// <param name="obj">Object to get the plural display name for - typically an enum value.</param>
        /// <returns>The plural display name of the object.</returns>
        public static string PluralDisplayName(this object obj)
        {
            if (obj == null)
                return "All";
            return obj.ToString() + 's';
        }
    }
}