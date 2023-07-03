using System;
using System.Collections.Generic;

namespace NorskaLib.Extensions
{
    public interface IStringIdProvider
    {
        string Id { get; }
    }

    public interface IIntegerIdProvider
    {
        int Id { get; }
    }

    public static class IEnumerableExtensions
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> collection)
        {
            var queue = new Queue<T>();
            foreach (var item in collection)
                queue.Enqueue(item);

            return queue;
        }

        public static bool TryGet<T>(this IEnumerable<T> collection, Func<T, bool> predicate, out T result)
        {
            foreach (var item in collection)
                if (predicate(item))
                {
                    result = item;
                    return true;
                }

            result = default;
            return false;
        }

        public static bool TryGet<T>(this IEnumerable<T> collection, string id, out T result) where T : IStringIdProvider
        {
            foreach (var item in collection)
                if (item.Id == id)
                {
                    result = item;
                    return true;
                }

            result = default;
            return false;
        }

        public static bool TryGet<T>(this IEnumerable<T> collection, int id, out T result) where T : IIntegerIdProvider
        {
            foreach (var item in collection)
                if (item.Id == id)
                {
                    result = item;
                    return true;
                }

            result = default;
            return false;
        }
    }
}