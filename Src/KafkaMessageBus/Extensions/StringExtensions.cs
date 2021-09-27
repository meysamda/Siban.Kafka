using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class StringExtensions
    {
        public static string GetString(this IEnumerable<string> query, string seperator = ", ")
        {
            if (query.Count() == 0) return null;

            return string.Join(seperator, query);
        }
    }
}