using GraphLinqQL;
using GraphLinqQL.Introspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GraphQlOptionsExtensions
    {
        public static void AddIntrospection(this GraphQlOptions options)
        {
            options.Query = typeof(IntrospectionQuery<>).MakeGenericType(options.Query!);
        }
    }
}
