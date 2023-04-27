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
    }
}