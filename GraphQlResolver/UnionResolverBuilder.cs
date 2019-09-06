using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQlResolver
{
    internal class UnionResolverBuilder : IComplexResolverBuilder<IGraphQlResolvable, IEnumerable<IDictionary<string, object>>>
    {

        private static readonly MethodInfo QueryableUnion = typeof(System.Linq.Queryable).GetMethods()
                    .Where(m => m.Name == nameof(System.Linq.Queryable.Union))
                    .Where(m => m.GetParameters().Length == 2)
                    .Select(m => m.MakeGenericMethod(typeof(IDictionary<string, object>)))
                    .Single();

        private ImmutableList<IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>>> resolvers;

        public UnionResolverBuilder(IUnionGraphQlResult<IEnumerable<IGraphQlResolvable>> unionResult)
            : this(unionResult.Results.Select(result => result.ResolveComplex()))
        {
        }

        public UnionResolverBuilder(IEnumerable<IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>>> resolvers)
        {
            this.resolvers = resolvers.ToImmutableList();
        }

        public IComplexResolverBuilder<IGraphQlResolvable, IEnumerable<IDictionary<string, object>>> Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult<object?>> resolve)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(displayName, resolve)));
        }

        public IComplexResolverBuilder<IGraphQlResolvable, IEnumerable<IDictionary<string, object>>> Add(string property, IDictionary<string, object?>? parameters = null)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, parameters)));
        }

        public IComplexResolverBuilder<IGraphQlResolvable, IEnumerable<IDictionary<string, object>>> Add(string displayName, string property, IDictionary<string, object?>? parameters = null)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(displayName, property, parameters)));
        }

        public IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>> Add(string displayName, Func<IGraphQlResolvable, IGraphQlResult> resolve)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(displayName, resolve)));
        }

        public IGraphQlResult<IEnumerable<IDictionary<string, object>>> Build()
        {
            var results = resolvers.Select(r => r.Build()).ToArray();
            var param = results[0].UntypedResolver.Parameters[0];
            var expressions = results.Select(e => e.UntypedResolver.Body.Replace(e.UntypedResolver.Parameters[0], param)).ToArray();
            var lambda = Expression.Lambda(expressions.Skip(1).Aggregate(expressions[0], (prev, next) => Expression.Call(QueryableUnion, prev, next)), param);
            return new GraphQlExpressionResult<IEnumerable<IDictionary<string, object>>>(lambda, results[0].ServiceProvider);
        }

        public IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>> IfType(string value, Func<IComplexResolverBuilder<object>, IComplexResolverBuilder<object>> typedBuilder)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.IfType(value, typedBuilder)));
        }

        IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>> IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>>.Add(string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, parameters)));
        }

        IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>> IComplexResolverBuilder<IEnumerable<IDictionary<string, object>>>.Add(string displayName, string property, IDictionary<string, object?>? parameters)
        {
            return new UnionResolverBuilder(resolvers.Select(r => r.Add(property, property, parameters)));
        }
    }
}