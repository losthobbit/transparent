using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Return a random item from a list.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="list">List from which to retrieve a random item</param>
        /// <returns>A random item</returns>
        public static T Random<T>(this IEnumerable<T> list)
        {
            var count = list.Count(); // 1st round-trip
            var index = new Random().Next(count);
            return list.Skip(index).FirstOrDefault(); // 2nd round-trip
        }
    }
}
