using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQlResolver
{
    internal class GraphQlResultFactory<TValue> : IGraphQlResultFactory<TValue>
    {
        private readonly IServiceProvider serviceProvider;
        public GraphQlResultFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        IGraphQlResultJoinedFactory<TValue, TJoinedType> IGraphQlResultFactory<TValue>.Join<TJoinedType>(GraphQlJoin<TValue, TJoinedType> join)
        {
            return new GraphQlResultJoinedFactory<TValue, TJoinedType>(join);
        }

        IGraphQlResult<TDomainResult> IGraphQlResultFactory<TValue>.Resolve<TDomainResult>(Expression<Func<TValue, TDomainResult>> resolver)
        {
            return GraphQlExpressionResult<TDomainResult>.Construct<TValue>(resolver, serviceProvider);
        }

    }

    // TODO - can this be removed?
    internal class GraphQlComplexResult<TContract, TModel> : IGraphQlResult<TContract>
    {
        public GraphQlComplexResult(LambdaExpression resolver, IServiceProvider serviceProvider)
        {
            this.UntypedResolver = resolver;
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public LambdaExpression UntypedResolver { get; }

    }

}