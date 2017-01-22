using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public static class EnumerableExtensions
    {
        public static TOut[] AsArray<TIn, TOut>(this IEnumerable items, Func<TIn, TOut> fn)
        {
            return items.Cast<TIn>().Select(x => fn(x)).ToArray();
        }
    }

    public static class ObjectExtensions
    {
        public static TResult? _<T, TResult>(this T obj, Func<T, TResult?> fn)
            where TResult : struct
        {
            return obj == null ? (TResult?)null : fn(obj);
        }

        public static TResult _<T, TResult>(this T obj, Func<T, TResult> fn)
            where TResult : class
        {
            return obj == null ? null : fn(obj);
        }

        public static TResult? _S<T, TResult>(this T obj, Func<T, TResult> fn)
            where TResult : struct
        {
            return obj == null ? (TResult?)null : fn(obj);
        }

        public static T? Nullable<T>(this T obj)
            where T : struct
        {
            return obj;
        }
    }

    public static class ControlExtensions
    {
        public static void SetItems<T>(this ComboBox cb, T[] array)
        {
            cb.BeginUpdate();
            try
            {
                var oldSelected = cb.SelectedItem;
                cb.Items.Clear();
                cb.Items.AddRange(array.Cast<object>().ToArray());
                cb.SelectedItem = oldSelected;
            }
            finally
            {
                cb.EndUpdate();
            }
        }

        public static void SetItems<T>(this ListBox lb, T[] array)
        {
            lb.BeginUpdate();
            try
            {
                var oldSelected = lb.SelectedItem;
                lb.Items.Clear();
                lb.Items.AddRange(array.Cast<object>().ToArray());
                lb.SelectedItem = oldSelected;
            }
            finally
            {
                lb.EndUpdate();
            }
        }
    }
}