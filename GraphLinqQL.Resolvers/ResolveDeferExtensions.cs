using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    public static class ResolveDeferExtensions
    {

        public static IGraphQlScalarResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlScalarResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.ConstructResult();

            return original.UpdatePreambleAndBody<TContract>(preambleLambda =>
            {
                var replacement = new PreambleReplacement(Expression.Lambda(Deferred.newPreamble.Inline(original.Body.Parameters[0].Box(), PreamblePlaceholders.BodyPlaceholderExpression), original.Body.Parameters));
                var result = replacement.Replace(preambleLambda);
                return result;
            }, deferredLambda => constructedDeferred);
        }

        public static IGraphQlObjectResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.Resolution.ConstructResult();

            var newScalar = original.UpdatePreambleAndBody<object>(preambleLambda =>
            {
                var replacement = new PreambleReplacement(Expression.Lambda(Deferred.newPreamble.Inline(original.Body.Parameters[0].Box(), PreamblePlaceholders.BodyPlaceholderExpression), original.Body.Parameters));
                var result = replacement.Replace(preambleLambda);
                return result;
            }, deferredLambda => constructedDeferred).AddPostBuild(built =>
            {
                var result = (LambdaExpression)new DeferredVisitor().Visit(built);
                return result;
            });
            return newResult.AdjustResolution<TContract>(_ => newScalar);
        }

        class Deferred
        {
            public static readonly Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Invoke(new Deferred(deferFunction), input);
            public static readonly ConstructorInfo constructor = typeof(Deferred).GetConstructors().Single();

            private readonly LambdaExpression deferFunction;

            public Deferred(LambdaExpression deferFunction)
            {
                this.deferFunction = deferFunction;
            }

            public static object Invoke(Deferred deferred, object input)
            {
                return Execution.GraphQlResultExtensions.InvokeExpression(input, deferred.deferFunction);
            }
        }

        class DeferredVisitor : ExpressionVisitor
        {
            protected override Expression VisitNew(NewExpression node)
            {
                if (node.Type == typeof(Deferred))
                {
                    var deferred = Expression.Lambda<Func<object>>(node).Compile()();
                    return Expression.Constant(deferred);
                }
                return base.VisitNew(node);
            }
        }
    }
}
