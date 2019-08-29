using GraphQlSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IQueryable<T> Query<T>() // TODO - this is a temp function for tests
        {
            return new Query<T>(new GraphQlQueryProvider());
        }


        public static IGraphQlComplexResult<T> GraphQlRoot<T>(this IServiceProvider serviceProvider)
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable, new() // TODO - don't require this new()
        {
            //return new GraphQlResolver<T>(new Query<GraphQlRoot>(new GraphQlQueryProvider()));
            return new GraphQlConstantResult<GraphQlRoot, T>(new GraphQlRoot(), serviceProvider);
        }

        public static IQueryable<TResult> FromMany<T, TResult>()
            where T : IResolutionFactory<IReadOnlyList<TResult>>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<TResult> AsGraphQl<T, TResult>(this IQueryable<T> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<IReadOnlyList<TResult>> AsGraphQl<T, TResult>(this IQueryable<IReadOnlyList<T>> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<IReadOnlyDictionary<string, object>> Combine<T>(this IQueryable<T> original, CombineOptions<T> combineOptions)
        {
            throw new NotImplementedException();
            //return new CombinationStrategy(combineOptions);
        }
    }
}
