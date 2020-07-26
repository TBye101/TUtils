using System;
using System.Collections.Generic;
using TUtils.Data_Structures;

namespace TUtils.Collections.Enumeration
{
    /// <summary>
    /// Adds a few useful extensions to collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Enables modification while iterating in reverse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action">An action to execute on each item in the collection. <typeof item in collection, index of item in collection></typeof></param>
        public static void ReverseIterate<T>(this IList<T> collection, Action<T, int> action)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                T item = collection[i];
                action.Invoke(item, i);
            }
        }

        /// <summary>
        /// Enables modification while iterating.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action">An action to execute on each item in the collection. <typeof item in collection, index of item in collection></typeof></param>
        public static void Iterate<T>(this IList<T> collection, Action<T, int> action)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                T item = collection[i];
                action.Invoke(item, i);
            }
        }

        /// <param name="enumerable"></param>
        /// <param name="action">An action to execute on each item in the collection.</param>
        /// <typeparam name="T"></typeparam>
        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action.Invoke(item);
            }
        }
    }
}