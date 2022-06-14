using System;
using System.Collections.Generic;

namespace Cafeo.Utils
{
    public static class CollectionExtension
    {
        private static readonly Random rng = new();

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[rng.Next(list.Count)];
        }
    }
}