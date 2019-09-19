using System;
using System.Collections.Generic;

namespace GraphQlResolver
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder Add(string property, IDictionary<string, string>? parameters = null);
        IComplexResolverBuilder Add(string displayName, string property, IDictionary<string, string>? parameters = null);
        IGraphQlResult Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }
}