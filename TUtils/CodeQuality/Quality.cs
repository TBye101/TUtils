using System;
using System.Diagnostics;

namespace TUtils.CodeQuality
{
    /// <summary>
    /// A class dedicated to defense coding helpers, such as assertions.
    /// </summary>
    public static class Quality
    {
        /// <summary>
        /// Throws an exception if the specified condition is false.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <typeparam name="T">The type of exception to throw should the condition be false.</typeparam>
        [Conditional("DEBUG")]
        public static void Require<T>(bool condition)
            where T : Exception
        {
            if (!condition)
            {
                ThrowException<T>();
            }
        }

        [Conditional("DEBUG")]
        public static void NotNull<T>(params object[] parameters)
            where T : Exception
        {
            foreach (object item in parameters)
            {
                if (item == null)
                    ThrowException<T>();
            }
        }

        /// <summary>
        /// Throws an exception given its type.
        /// </summary>
        /// <typeparam name="T">The type of the exception to throw.</typeparam>
        [Conditional("DEBUG")]
        private static void ThrowException<T>()
            where T : Exception
        {
            T exception = (T)Activator.CreateInstance(typeof(T));
            throw exception;
        }
    }
}