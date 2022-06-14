using System;
using System.Collections.Generic;

namespace Cafeo.Utils
{
    public static class EnumerableExtensions
    {
        public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
            where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            var first = true;
            var maxObj = default(T);
            var maxKey = default(U);
            foreach (var item in source)
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    var currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }

            if (first) throw new InvalidOperationException("Sequence is empty.");
            return maxObj;
        }
    }
}