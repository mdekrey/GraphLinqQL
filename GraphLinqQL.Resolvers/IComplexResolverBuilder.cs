using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlScalarResult<object>> resolve);
        IComplexResolverBuilder Add(string propertyName, FieldContext context, IGraphQlParameterResolver? parameters = null);
        IComplexResolverBuilder Add(string displayName, string propertyName, FieldContext context, IGraphQlParameterResolver? parameters = null);
        IGraphQlScalarResult<object> Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }
}