using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    internal class UnionResolverBuilder : IComplexResolverBuilder
    {

        private static readonly MethodInfo QueryableUnion = typeof(System.Linq.Queryable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Union))
                    .Where(m => m.GetParameters().Length == 2)
                    .Select(m => m.MakeGenericMethod(typeof(object)))
                    .Single();
        private readonly IGraphQlParameterResolverFactory parameterResolverFactory;
        private readonly ImmutableList<IComplexResolverBuilder> resolvers;

        public UnionResolverBuilder(IGraphQlParameterResolverFactory parameterResolverFactory, IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult, IServiceProvider serviceProvider)
            : this(parameterResolverFactory, unionResult.Results.Select(result => result.ResolveComplex(serviceProvider)))
        {
        }

        public UnionResolverBuilder(IGraphQlParameterResolverFactory parameterResolverFactory, IEnumerable<IComplexResolverBuilder> resolvers)
        {
            this.parameterResolverFactory = parameterResolverFactory;
            this.resolvers = resolvers.ToImmutableList();
        }

        public IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder(parameterResolverFactory, resolvers.Select(r => r.Add(displayName, resolve)));
        }

        public IGraphQlResult Build()
        {
            var results = resolvers.Select(r => r.Build()).ToArray();
            var param = results[0].UntypedResolver.Parameters[0];
            var expressions = results.Select(e => e.UntypedResolver.Inline(param)).ToArray();
            var lambda = Expression.Lambda(expressions.Skip(1).Aggregate(expressions[0], (prev, next) => Expression.Call(QueryableUnion, prev, next)), param);
            return new GraphQlExpressionResult<object>(parameterResolverFactory, lambda);
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            return new UnionResolverBuilder(parameterResolverFactory, resolvers.Select(r => r.IfType(value, typedBuilder)));
        }

        public IComplexResolverBuilder Add(string property, IDictionary<string, string>? parameters)
        {
            return new UnionResolverBuilder(parameterResolverFactory, resolvers.Select(r => r.Add(property, parameters)));
        }

        public IComplexResolverBuilder Add(string displayName, string property, IDictionary<string, string>? parameters)
        {
            return new UnionResolverBuilder(parameterResolverFactory, resolvers.Select(r => r.Add(property, property, parameters)));
        }
    }
}