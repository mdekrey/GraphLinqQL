using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    internal class MultiObjectComplexResolver : IComplexResolverBuilder
    {
        private IReadOnlyList<IComplexResolverBuilder> originals;
        private readonly Func<IReadOnlyList<LambdaExpression>, LambdaExpression> combineResults;
        private readonly IReadOnlyCollection<IGraphQlJoin> joins;

        public MultiObjectComplexResolver(IComplexResolverBuilder[] originals, Func<IReadOnlyList<LambdaExpression>, LambdaExpression> combineResults, IReadOnlyCollection<IGraphQlJoin> joins)
        {
            this.originals = originals;
            this.combineResults = combineResults;
            this.joins = joins;
        }

        public IComplexResolverBuilder Add(string displayName, FieldContext context, Func<IGraphQlResolvable, IGraphQlScalarResult> resolve) =>
            new MultiObjectComplexResolver(originals.Select(original => original.Add(displayName, context, resolve)).ToArray(), combineResults, joins);

        public IComplexResolverBuilder Add(string propertyName, FieldContext context, IGraphQlParameterResolver? parameters = null) =>
            new MultiObjectComplexResolver(originals.Select(original => original.Add(propertyName, context, parameters)).ToArray(), combineResults, joins);

        public IComplexResolverBuilder Add(string displayName, string propertyName, FieldContext context, IGraphQlParameterResolver? parameters = null) =>
            new MultiObjectComplexResolver(originals.Select(original => original.Add(displayName, propertyName, context, parameters)).ToArray(), combineResults, joins);

        public IGraphQlScalarResult Build()
        {
            var scalars = originals.Select(original => original.Build().ConstructResult()).ToArray();
            var result = combineResults(scalars);
            return new GraphQlExpressionScalarResult<object>(result, (Expression<Func<object, object>>)(_ => _), joins);
        }

        public IComplexResolverBuilder IfType(string value, Func<IComplexResolverBuilder, IComplexResolverBuilder> typedBuilder) =>
            new MultiObjectComplexResolver(originals.Select(original => original.IfType(value, typedBuilder)).ToArray(), combineResults, joins);
    }
}