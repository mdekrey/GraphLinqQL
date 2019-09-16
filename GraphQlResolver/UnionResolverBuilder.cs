using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    internal class UnionResolverBuilder<TContract> : IComplexResolverBuilder<TContract>
        where TContract : IGraphQlResolvable
    {

        private static readonly MethodInfo QueryableUnion = typeof(System.Linq.Queryable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Union))
                    .Where(m => m.GetParameters().Length == 2)
                    .Select(m => m.MakeGenericMethod(typeof(IDictionary<string, object>)))
                    .Single();

        private ImmutableList<IComplexResolverBuilder> resolvers;

        public UnionResolverBuilder(IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult)
            : this(unionResult.Results.Select(result => result.ResolveComplex()))
        {
        }

        public UnionResolverBuilder(IEnumerable<IComplexResolverBuilder> resolvers)
        {
            this.resolvers = resolvers.ToImmutableList();
        }

        public IComplexResolverBuilder<TContract> Add(string displayName, Func<TContract, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(displayName, r => resolve((TContract)r))));
        }

        public IComplexResolverBuilder<TContract> Add(string property, IDictionary<string, object?>? parameters = null)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(property, parameters)));
        }

        public IComplexResolverBuilder<TContract> Add(string displayName, string property, IDictionary<string, object?>? parameters = null)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(displayName, property, parameters)));
        }

        public IComplexResolverBuilder Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(displayName, resolve)));
        }

        public IGraphQlResult Build()
        {
            var results = resolvers.Select(r => r.Build()).ToArray();
            var param = results[0].UntypedResolver.Parameters[0];
            var expressions = results.Select(e => e.UntypedResolver.Body.Replace(e.UntypedResolver.Parameters[0], param)).ToArray();
            var lambda = Expression.Lambda(expressions.Skip(1).Aggregate(expressions[0], (prev, next) => Expression.Call(QueryableUnion, prev, next)), param);
            return new GraphQlExpressionResult<object>(lambda, results[0].ServiceProvider);
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.IfType(value, typedBuilder)));
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(property, parameters)));
        }

        IComplexResolverBuilder IComplexResolverBuilder.Add(string displayName, string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder<TContract>(resolvers.Select(r => r.Add(property, property, parameters)));
        }
    }
}