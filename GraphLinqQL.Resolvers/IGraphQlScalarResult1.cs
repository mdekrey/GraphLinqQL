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

        // FIXME - if we can not expose these as interface members it would be better
        LambdaExpression Body { get; }

        IReadOnlyCollection<IGraphQlJoin> Joins { get; }
        IGraphQlObjectResult<T> AsContract<T>(IContract contract, Func<Expression, Expression> bodyWrapper);

        IGraphQlScalarResult AddPostBuild(Func<LambdaExpression, LambdaExpression> postBuild);

        // TODO - should prefer this to preamble/body
        //IGraphQlScalarResult<T> UpdateCurrent<T>(Func<LambdaExpression, LambdaExpression> resolveAdjust);
        IGraphQlScalarResult<T> UpdatePreamble<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust);
        IGraphQlScalarResult<T> UpdateBody<T>(Func<LambdaExpression, LambdaExpression> bodyAdjust);
        IGraphQlScalarResult<T> UpdatePreambleAndBody<T>(Func<LambdaExpression, LambdaExpression> preambleAdjust, Func<LambdaExpression, LambdaExpression> bodyAdjust);
    }

}
