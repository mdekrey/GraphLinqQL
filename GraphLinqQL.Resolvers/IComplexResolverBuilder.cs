using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder Add(string propertyName, FieldContext context, IDictionary<string, IGraphQlParameterInfo>? parameters = null);
        IComplexResolverBuilder Add(string displayName, string propertyName, FieldContext context, IDictionary<string, IGraphQlParameterInfo>? parameters = null);
        IGraphQlResult Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }
}