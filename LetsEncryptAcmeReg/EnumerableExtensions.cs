using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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

        [NotNull]
        public static IEnumerable<T> Sort<T>([CanBeNull] this IEnumerable<T> items)
        {
            return items?.OrderBy(x => x) ?? Enumerable.Empty<T>();
        }

        [NotNull]
        public static IEnumerable<T> Append<T>([CanBeNull] this IEnumerable<T> items, T newItem)
        {
            if (items != null)
                foreach (var item in items)
                    yield return item;

            yield return newItem;
        }

        [NotNull]
        public static IEnumerable<T> Append<T>([CanBeNull] this IEnumerable<T> items, [CanBeNull] params T[] newItems)
        {
            if (items != null)
                foreach (var item in items)
                    yield return item;

            if (newItems != null)
                foreach (var newItem in newItems)
                    yield return newItem;
        }

        [NotNull]
        public static IEnumerable<T> Prepend<T>([CanBeNull] this IEnumerable<T> items, T newItem)
        {
            yield return newItem;

            if (items != null)
                foreach (var item in items)
                    yield return item;
        }

        [NotNull]
        public static IEnumerable<T> Prepend<T>([CanBeNull] this IEnumerable<T> items, [CanBeNull] params T[] newItems)
        {
            if (newItems != null)
                for (int index = 0; index < newItems.Length; index++)
                {
                    var newItem = newItems[newItems.Length - index - 1];
                    yield return newItem;
                }

            if (items != null)
                foreach (var item in items)
                    yield return item;
        }

        private static bool IsNotNullOrWhiteSpace(string str) => !string.IsNullOrWhiteSpace(str);
        private static bool IsNullOrWhiteSpace(string str) => string.IsNullOrWhiteSpace(str);
        private static bool IsNull<T>(T str) => str == null;
        private static bool IsNotNull<T>(T str) => str != null;
    }
}