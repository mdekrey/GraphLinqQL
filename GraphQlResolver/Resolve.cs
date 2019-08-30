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
            IGraphQlResultFactory<GraphQlRoot> resultFactory = new GraphQlResultFactory<GraphQlRoot>(serviceProvider);
            var resolved = resolver(resultFactory.Resolve(a => a).As<T>().ResolveComplex());
            return resolved?.Resolve<GraphQlRoot>().Compile()(new GraphQlRoot());
        }
    }
}
