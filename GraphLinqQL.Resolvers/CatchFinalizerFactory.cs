using GraphLinqQL.Resolution;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL
{
    public class CatchFinalizerFactory
    {
        public static readonly MethodInfo CatchMethodInfo = typeof(CatchFinalizerFactory).GetMethod(nameof(Catch));
        private readonly FieldContext fieldContext;
        private readonly LambdaExpression expression;

        public CatchFinalizerFactory(FieldContext fieldContext, [ExtractLambda] LambdaExpression expression)
        {
            this.fieldContext = fieldContext;
            this.expression = expression;
        }

        public IFinalizer Catch(Func<object?> valueAccessor)
        {
            return new CatchFinalizer(fieldContext, valueAccessor, expression);
        }
    }
}
