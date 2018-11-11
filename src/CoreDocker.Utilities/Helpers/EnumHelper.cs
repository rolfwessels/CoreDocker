using System;
using System.Collections.Generic;

namespace CoreDocker.Utilities.Helpers
{
    public class EnumHelper
    {
        public static IEnumerable<T> ToArray<T>()
        {
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values) yield return (T) value;
        }
    }
}