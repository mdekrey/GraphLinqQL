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
                    .Select(m => m.MakeGenericMethod(typeof(IDictionary<string, object>)))
                    .Single();

        private ImmutableList<IComplexResolverBuilder> resolvers;

        public UnionResolverBuilder(IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult, IServiceProvider serviceProvider)
            : this(unionResult.Results.Select(result => result.ResolveComplex(serviceProvider)))
        {
        }

        public UnionResolverBuilder(IEnumerable<IComplexResolverBuilder> resolvers)
        {
            this.resolvers = resolvers.ToImmutableList();
        }

        public IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(displayName, resolve)));
        }

        public IGraphQlResult Build()
        {
            var results = resolvers.Select(r => r.Build()).ToArray();
            var param = results[0].UntypedResolver.Parameters[0];
            var expressions = results.Select(e => e.UntypedResolver.Body.Replace(e.UntypedResolver.Parameters[0], param)).ToArray();
            var lambda = Expression.Lambda(expressions.Skip(1).Aggregate(expressions[0], (prev, next) => Expression.Call(QueryableUnion, prev, next)), param);
            return new GraphQlExpressionResult<object>(lambda);
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.IfType(value, typedBuilder)));
        }

        public IComplexResolverBuilder Add(string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, parameters)));
        }

        public IComplexResolverBuilder Add(string displayName, string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, property, parameters)));
        }
    }
}