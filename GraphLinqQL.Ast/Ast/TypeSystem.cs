using System;
using System.Collections.Generic;
using System.Reflection;

namespace GraphLinqQL
{
    internal static class TypeSystem
    {
        public static Type GetElementType(Type seqType)
        {
            Type? ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetTypeInfo().GenericTypeArguments[0];
        }
        private static Type? FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            var seqTypeInfo = seqType.GetTypeInfo();
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType()!);
            if (seqTypeInfo.IsGenericType)
            {
                foreach (Type arg in seqTypeInfo.GenericTypeArguments)
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.GetTypeInfo().IsAssignableFrom(seqTypeInfo))
                    {
                        return ienum;
                    }
                }
            }
            IEnumerable<Type> ifaces = seqTypeInfo.ImplementedInterfaces;
            if (ifaces != null)
            {
                foreach (Type iface in ifaces)
                {
                    Type? ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqTypeInfo.BaseType != null && seqTypeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(seqTypeInfo.BaseType);
            }
            return null;
        }
    }
}