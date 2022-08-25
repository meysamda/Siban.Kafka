using System.Linq;

namespace System.Collections.Generic
{
    internal static class IEnumerableExtensions
    {
        public static string ToSepratedString(this IEnumerable<string> items, string seperator = ", ")
        {
            if (items.Count() == 0) return null;

            return string.Join(seperator, items);
        }
    }
}