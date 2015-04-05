using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class StringRoutines
    {
        public static string Repeat(this string stringToRepeat, int count)
        {
            return String.Concat(Enumerable.Range(0, count).Select(x => stringToRepeat));
        }

        public static string Spaces(int numberOfSpaces)
        {
            return new String(' ', numberOfSpaces);
        }

        /// <summary>
        /// Prefixes with a or an.
        /// </summary>
        public static string PrefixIndefiniteArticle(this string noun)
        {
            return ("aeiou".Contains(Char.ToLower(noun[0])) ? "an" : "a") + ' ' + noun;
        }

        public static string CapitalizeFirstLetter(this string text)
        {
            return Char.ToUpper(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Converts camel case into end user readable text.
        /// </summary>
        public static string CamelCaseToSpacedWords(this string camelCase)
        {
            return Regex.Replace(
                Regex.Replace(
                    camelCase,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }
}
