using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class StringRoutines
    {
        public static string Repeat(string stringToRepeat, int count)
        {
            return String.Concat(Enumerable.Range(0, count).Select(x => stringToRepeat));
        }

        public static string Spaces(int numberOfSpaces)
        {
            return new String(' ', numberOfSpaces);
        }
    }
}
