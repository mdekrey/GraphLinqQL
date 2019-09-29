using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder Add(string propertyName, IDictionary<string, IGraphQlParameterInfo>? parameters = null);
        IComplexResolverBuilder Add(string displayName, string propertyName, IDictionary<string, IGraphQlParameterInfo>? parameters = null);
        IGraphQlResult Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }
}