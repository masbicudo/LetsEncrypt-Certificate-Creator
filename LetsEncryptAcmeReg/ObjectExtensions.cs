using System;

namespace LetsEncryptAcmeReg
{
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
}