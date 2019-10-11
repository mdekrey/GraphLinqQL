using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    internal class UnionResolverBuilder : IComplexResolverBuilder
    {
        // due to efcore 16243, I'm downgrading this to use the Enumerable union
        private static readonly MethodInfo QueryableUnion = typeof(System.Linq.Enumerable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Union))
                    .Where(m => m.GetParameters().Length == 2)
                    .Select(m => m.MakeGenericMethod(typeof(object)))
                    .Single();
        private readonly ImmutableList<IComplexResolverBuilder> resolvers;

        public UnionResolverBuilder(IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult, IGraphQlServiceProvider serviceProvider, FieldContext queryContext)
            : this(unionResult.Results.Select(result => result.ResolveComplex(serviceProvider, queryContext)))
        {
        }

        public UnionResolverBuilder(IEnumerable<IComplexResolverBuilder> resolvers)
        {
            this.resolvers = resolvers.ToImmutableList();
        }

        public IComplexResolverBuilder Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(displayName, context, resolve)));
        }

        public IGraphQlScalarResult Build()
        {
            var results = resolvers.Select(r => r.Build()).ToArray();
            var param = results[0].UntypedResolver.Parameters[0];
            var expressions = results.Select(e => e.UntypedResolver.Inline(param)).ToArray();
            var lambda = Expression.Lambda(expressions.Skip(1).Aggregate(expressions[0], (prev, next) => Expression.Call(QueryableUnion, prev, next)), param);
            return new GraphQlExpressionScalarResult<object>(lambda, EmptyArrayHelper.Empty<IGraphQlJoin>());
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.IfType(value, typedBuilder)));
        }

        public IComplexResolverBuilder Add(string property, FieldContext context, IGraphQlParameterResolver? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, context, parameters)));
        }

        public IComplexResolverBuilder Add(string displayName, string property, FieldContext context, IGraphQlParameterResolver? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, property, context, parameters)));
        }
    }
}