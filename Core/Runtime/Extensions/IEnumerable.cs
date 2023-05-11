using System;
using System.Collections.Generic;

namespace NorskaLib.Extensions
{
    public static class IEnumerableExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> collection)
        {
            var queue = new Queue<T>();
            foreach (var item in collection)
                queue.Enqueue(item);

            return queue;
        }

        public static bool TryGet<T>(this IEnumerable<T> collection, Func<T, bool> predicate, out T item)
        {
            foreach (var i in collection)
                if (predicate(i))
                {
                    item = i;
                    return true;
                }

            item = default;
            return false;
        }
    }
}