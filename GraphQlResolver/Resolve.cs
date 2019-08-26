using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IQueryable<TResult> From<T, TResult>() where T : IResolutionFactory<TResult>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult>();
        }

        public static IQueryable<TResult> FromMany<T, TResult>() where T : IResolutionFactory<IList<TResult>>
        {
            throw new NotImplementedException();
            //return new MultiResolutionStrategy<T, TResult>();
        }

        public static IQueryable<TResult> From<T, TResult, TKey>(IQueryable<TKey> keyStrategy) where T : IResolutionFactory<TResult, TKey>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }
        public static IQueryable<TResult> FromMany<T, TResult, TKey>(IQueryable<TKey> keyStrategy) where T : IResolutionFactory<IList<TResult>, TKey>
        {
            throw new NotImplementedException();
            //return new MultiResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IQueryable<object> Combine(CombineOptions combineOptions)
        {
            throw new NotImplementedException();
            //return new CombinationStrategy(combineOptions);
        }
    }
}
