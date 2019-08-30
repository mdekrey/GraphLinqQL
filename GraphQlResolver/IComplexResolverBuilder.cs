using System;

namespace GraphQlResolver
{
    public interface IComplexResolverBuilder<out TContract, out TFinal>
        where TContract : IGraphQlResolvable
        where TFinal : IGraphQlResult
    {
        IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult> resolve);
        IComplexResolverBuilder<TContract, TFinal> Add(string property, params object[] parameters);
        IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, params object[] parameters);
        TFinal Build();
    }
}