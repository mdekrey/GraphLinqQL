using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    class GraphQlExpressionObjectResult<TReturnType> : IGraphQlObjectResult<TReturnType>
    {
        private readonly GraphQlContractExpressionReplaceVisitor visitor;

        public GraphQlExpressionObjectResult(
            IGraphQlScalarResult resolution,
            IContract contract)
        {
            this.Resolution = resolution;
            this.Contract = contract ?? throw new ArgumentException("Expected a contract but had none.", nameof(resolution));

            visitor = new GraphQlContractExpressionReplaceVisitor();
            visitor.Visit(resolution.Body);
            if (visitor.ModelType == null)
            {
                throw new ArgumentException("The provided resolver did not have a contract.", nameof(resolution));
            }
        }

        public IContract Contract { get; }

        public IGraphQlScalarResult Resolution { get; }

        public IGraphQlObjectResult<T> AdjustResolution<T>(Func<IGraphQlScalarResult, IGraphQlScalarResult> p)
        {
            return new GraphQlExpressionObjectResult<T>(p(Resolution), Contract);
        }

        public IComplexResolverBuilder ResolveComplex(IGraphQlServiceProvider serviceProvider)
        {
            return new ComplexResolverBuilder(
                Contract!,
                serviceProvider,
                ToResult,
                visitor.ModelType!
            );
        }

        private IGraphQlScalarResult<object> ToResult(IReadOnlyList<LambdaExpression> joinedSelector)
        {
            visitor.NewOperations = joinedSelector;
            return Resolution.UpdateBody<object>(body =>
            {
                var returnResult = visitor.Visit(body.Body);
                return Expression.Lambda(returnResult, body.Parameters);
            });
        }

    }
}