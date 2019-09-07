using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver.Introspection
{
    static class IntrospectionTypeExtensions
    {
        public static IGraphQlTypeInformation? MaybeInstantiate(this IServiceProvider serviceProvider, Type? t)
        {
            return t == null
                ? null
                : (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, t);
        }

        public static IGraphQlTypeInformation Instantiate(this IServiceProvider serviceProvider, Type t)
        {
            return (IGraphQlTypeInformation)ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, t);
        }

    }
}
