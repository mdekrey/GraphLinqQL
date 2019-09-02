using System;
using System.Collections.Generic;

namespace GraphQlResolver
{
    public interface IComplexResolverBuilder<out TFinal>
    {
        IComplexResolverBuilder<TFinal> Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder<TFinal> Add(string property, IDictionary<string, object>? parameters = null);
        IComplexResolverBuilder<TFinal> Add(string displayName, string property, IDictionary<string, object>? parameters = null);
        IGraphQlResult<TFinal> Build();
        bool IsType(string value);
    }

    public interface IComplexResolverBuilder<out TContract, out TFinal> : IComplexResolverBuilder<TFinal>
        where TContract : IGraphQlResolvable
    {
        IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult<object>> resolve);
        new IComplexResolverBuilder<TContract, TFinal> Add(string property, IDictionary<string, object>? parameters = null);
        new IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, IDictionary<string, object>? parameters = null);
    }
}