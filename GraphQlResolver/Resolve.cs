using System;
using System.Collections.Generic;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IResolutionStrategy<TResult> From<T, TResult>() where T : IResolutionFactory<TResult>
        {
            return new SingleResolutionStrategy<T, TResult>();
        }

        public static IResolutionStrategy<TResult> FromMany<T, TResult>() where T : IResolutionFactory<IList<TResult>>
        {
            return new MultiResolutionStrategy<T, TResult>();
        }

        public static IResolutionStrategy<TResult> From<T, TResult, TKey>(IResolutionStrategy<TKey> keyStrategy) where T : IResolutionFactory<TResult, TKey>
        {
            return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }
        public static IResolutionStrategy<TResult> FromMany<T, TResult, TKey>(IResolutionStrategy<TKey> keyStrategy) where T : IResolutionFactory<IList<TResult>, TKey>
        {
            return new MultiResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IResolutionStrategy<object> Combine(CombineOptions combineOptions)
        {
            return new CombinationStrategy(combineOptions);
        }
    }
}
