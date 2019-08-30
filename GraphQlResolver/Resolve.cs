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

        public static object? GraphQlRoot<T>(this IServiceProvider serviceProvider, Func<IComplexResolverBuilder<T, object>, IGraphQlResult<object>> resolver)
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable
        {
            var root = new GraphQlRoot();
            var resolved = resolver(new GraphQlExpressionResult<GraphQlRoot, GraphQlRoot>(a => a, serviceProvider).As<T>()) as IGraphQlResultFromInput<GraphQlRoot>;
            return resolved?.Resolve().Compile()(root);
        }

        public static IGraphQlResult<object> Box<T>(this IGraphQlResult<T> input)
        {
            throw new NotImplementedException();
        }
    }
}
