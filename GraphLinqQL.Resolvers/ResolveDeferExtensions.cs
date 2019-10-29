using GraphLinqQL.Resolution;
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

            return original.AddResolve<TContract>(constructedDeferred);
        }

        public static IGraphQlObjectResult<TContract> Defer<TInput, TContract>(this IGraphQlScalarResult<TInput> original, Func<IGraphQlResultFactory<TInput>, IGraphQlObjectResult<TContract>> func)
        {
            var newResult = func(new GraphQlResultFactory<TInput>(original.FieldContext));
            var constructedDeferred = newResult.Resolution
                .ConstructResult();

            var param = Expression.Parameter(typeof(object), "inputForDeferred");
            var deferResolver = Expression.Lambda(Deferred.newPreamble.Inline(param, PreamblePlaceholders.BodyPlaceholderExpression), param);
            var outParam = Expression.Parameter(typeof(object), "outputForDeferred");

            var newScalar = original.AddResolve<object>(deferResolver)
                .AddResolve<object>(Expression.Lambda(Expression.Convert(outParam, constructedDeferred.Parameters[0].Type), outParam))
                .AddResolve<object>(constructedDeferred);
            return newResult.AdjustResolution<TContract>(_ => newScalar);
        }
        class Deferred
        {
            public static readonly Expression<Func<object, LambdaExpression, object?>> newPreamble = (input, deferFunction) => Invoke(new Deferred(deferFunction), input);
            public static readonly ConstructorInfo constructor = typeof(Deferred).GetConstructors().Single();

            private readonly LambdaExpression deferFunction;

            public Deferred([ExtractLambda] LambdaExpression deferFunction)
            {
                this.deferFunction = deferFunction;
            }

            public static object Invoke(Deferred deferred, object input)
            {
                return Execution.GraphQlResultExtensions.InvokeExpression(input, deferred.deferFunction);
            }
        }
    }
}
