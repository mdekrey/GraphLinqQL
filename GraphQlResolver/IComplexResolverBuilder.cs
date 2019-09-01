using System;

namespace GraphQlResolver
{
    public interface IComplexResolverBuilder<out TFinal>
    {
        IComplexResolverBuilder<TFinal> Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve);
        IComplexResolverBuilder<TFinal> Add(string property, params object[] parameters);
        IComplexResolverBuilder<TFinal> Add(string displayName, string property, params object[] parameters);
        IGraphQlResult<TFinal> Build();
    }

    public interface IComplexResolverBuilder<out TContract, out TFinal> : IComplexResolverBuilder<TFinal>
        where TContract : IGraphQlResolvable
    {
        IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult<object>> resolve);
        new IComplexResolverBuilder<TContract, TFinal> Add(string property, params object[] parameters);
        new IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, params object[] parameters);
    }
}