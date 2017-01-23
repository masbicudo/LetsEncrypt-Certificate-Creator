using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LetsEncryptAcmeReg
{
    public static class EnumerableExtensions
    {
        public static TOut[] AsArray<TIn, TOut>(this IEnumerable items, Func<TIn, TOut> fn)
        {
            return items.Cast<TIn>().Select(fn).ToArray();
        }

        public static bool AnyNullOrWhiteSpace(this IEnumerable<string> items)
        {
            return items.Any(IsNullOrWhiteSpace);
        }

        public static bool AnyNotNullOrWhiteSpace(this IEnumerable<string> items)
        {
            return items.Any(IsNotNullOrWhiteSpace);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T newItem)
        {
            foreach (var item in items)
                yield return item;
            yield return newItem;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, params T[] newItems)
        {
            foreach (var item in items)
                yield return item;
            foreach (var newItem in newItems)
                yield return newItem;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, T newItem)
        {
            yield return newItem;
            foreach (var item in items)
                yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, params T[] newItems)
        {
            for (int index = 0; index < newItems.Length; index++)
            {
                var newItem = newItems[newItems.Length - index - 1];
                yield return newItem;
            }
            foreach (var item in items)
                yield return item;
        }

        private static bool IsNotNullOrWhiteSpace(string str) => !string.IsNullOrWhiteSpace(str);
        private static bool IsNullOrWhiteSpace(string str) => string.IsNullOrWhiteSpace(str);
    }
}