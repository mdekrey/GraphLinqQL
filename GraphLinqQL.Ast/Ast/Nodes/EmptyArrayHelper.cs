using System;

namespace GraphLinqQL.Ast.Nodes
{
    public static class EmptyArrayHelper
    {
#if NET45
        private static class EmptyArray<T>
        {
#pragma warning disable CA1825 // this is the replacement implementation of Array.Empty<T>()
            internal static readonly T[] Value = new T[0];
#pragma warning restore CA1825
        }
#endif

        public static T[] Empty<T>()
        {
#if NET45
            return EmptyArray<T>.Value;
#else
            return Array.Empty<T>();
#endif
        }
    }
}