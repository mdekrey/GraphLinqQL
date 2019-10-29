using GraphLinqQL.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GraphLinqQL
{
    public interface IGraphQlScalarResult : IGraphQlResult
    {
        FieldContext FieldContext { get; }
        LambdaExpression ConstructResult();

        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
        IGraphQlObjectResult<T> AsContract<T>(IContract contract, LambdaExpression bodyWrapper);

        IGraphQlScalarResult<T> AddResolve<T>(Func<ParameterExpression, LambdaExpression> resolve);
        IGraphQlScalarResult<T> ApplyVisitor<T>(ExpressionVisitor visitor);
        IGraphQlScalarResult<T> AddConstructionVisitor<T>(ExpressionVisitor visitor);
    }

}
