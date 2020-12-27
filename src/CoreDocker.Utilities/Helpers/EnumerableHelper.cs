using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace CoreDocker.Utilities.Helpers
{
    public static class EnumerableHelper
    {
        public static string StringJoin(this IEnumerable<object> values, string separator = ", ")
        {
            if (values == null) return null;
            var array = values.Select(x => x.ToString()).ToArray();
            return Join(separator, array);
        }


        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T> call)
        {
            if (values == null) return null;
            foreach (var value in values) call(value);
            return values;
        }

        public static object LookupValidValue<T>(this IEnumerable<T> values, string call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            var valueTuples = values.Select(x => new Tuple<T,string>( x, x.ToString())).ToArray();
            var firstOrDefault = valueTuples.FirstOrDefault(x =>
                string.Equals(x.Item2, call, StringComparison.CurrentCultureIgnoreCase));
            if (firstOrDefault != null) return firstOrDefault.Item1;
            return null;
        }
    }

    
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }

}