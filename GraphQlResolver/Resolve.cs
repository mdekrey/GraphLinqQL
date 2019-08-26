using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IQueryable<TResult> AsGraphQl<T, TResult>(this IQueryable<T> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<IReadOnlyList<TResult>> AsGraphQl<T, TResult>(this IQueryable<IReadOnlyList<T>> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<TResult> From<T, TResult>() where T : IResolutionFactory<TResult>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult>();
        }

        public static IQueryable<TResult> FromMany<T, TResult>() where T : IResolutionFactory<IReadOnlyList<TResult>>
        {
            throw new NotImplementedException();
            //return new MultiResolutionStrategy<T, TResult>();
        }

        public static IQueryable<TResult> Via<T, TResult, TKey>(this IQueryable<TKey> keyStrategy) where T : IResolutionFactory<TResult, TKey>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }
        public static IQueryable<TResult> ViaMany<T, TResult, TKey>(IQueryable<TKey> keyStrategy) where T : IResolutionFactory<IReadOnlyList<TResult>, TKey>
        {
            throw new NotImplementedException();
            //return new MultiResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IQueryable<TResult> Via<T, TResult, TKey, TModel>(this IQueryable<TKey> keyStrategy) 
            where T : IResolutionFactory<TModel, TKey> 
            where TResult : IGraphQlAccepts<TModel>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IQueryable<IReadOnlyList<TResult>> ViaMany<T, TResult, TKey, TModel>(this IQueryable<TKey> keyStrategy)
            where T : IResolutionFactory<IReadOnlyList<TModel>, TKey>
            where TResult : IGraphQlAccepts<TModel>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IQueryable<object> Combine(CombineOptions combineOptions)
        {
            throw new NotImplementedException();
            //return new CombinationStrategy(combineOptions);
        }
    }
}
