using System;
using System.Linq;

namespace GraphQlResolver
{
    public interface IResolutionStrategy
    {
    }

    public interface IResolutionStrategy<out TResult> : IResolutionStrategy, IQueryable<TResult>
    {
    }

    public static class ResolutionStrategyExtensions
    {
        public static IResolutionStrategy<TNext> Then<TResult, TNext>(this IResolutionStrategy<TResult> original, Func<IResolutionStrategy<TResult>, IResolutionStrategy<TNext>> p)
        {
            throw new NotImplementedException();
        }


        public static IQueryable<TNext> Then<TResult, TNext>(this IQueryable<TResult> original, Func<IQueryable<TResult>, IQueryable<TNext>> p) => p(original);

    }
}