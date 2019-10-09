using GraphLinqQL.Introspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL
{
    public static class GraphQlOptionsExtensions
    {
        public static void AddIntrospection(this GraphQlOptions options)
        {
            options.Query = typeof(Introspection.IntrospectionQuery<>).MakeGenericType(options.Query!);
        }
    }
}
