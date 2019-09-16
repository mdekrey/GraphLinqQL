using System;
using System.Collections.Generic;

namespace GraphQlResolver
{
    public interface IComplexResolverBuilder
    {
        IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder Add(string property, IDictionary<string, object?>? parameters = null);
        IComplexResolverBuilder Add(string displayName, string property, IDictionary<string, object?>? parameters = null);
        IGraphQlResult Build();
        IComplexResolverBuilder IfType(string value, System.Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder);
    }

    public interface IComplexResolverBuilder<out TContract> : IComplexResolverBuilder
        where TContract : IGraphQlResolvable
    {
        IComplexResolverBuilder<TContract> Add(string displayName, Func<TContract, IGraphQlResult> resolve);
        new IComplexResolverBuilder<TContract> Add(string property, IDictionary<string, object?>? parameters = null);
        new IComplexResolverBuilder<TContract> Add(string displayName, string property, IDictionary<string, object?>? parameters = null);
    }
}