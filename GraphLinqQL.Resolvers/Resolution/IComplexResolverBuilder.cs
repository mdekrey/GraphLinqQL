using System;
using System.Collections.Generic;

namespace GraphLinqQL.Resolution
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, IReadOnlyList<QueryLocation> locations, Func<IGraphQlResolvable, IGraphQlScalarResult<object>> resolve);
        IComplexResolverBuilder Add(string propertyName, IReadOnlyList<QueryLocation> locations, IGraphQlParameterResolver? parameters = null);
        IComplexResolverBuilder Add(string displayName, string propertyName, IReadOnlyList<QueryLocation> locations, IGraphQlParameterResolver? parameters = null);
        IGraphQlScalarResult<object> Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }
}