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

        public CatchFinalizerFactory(FieldContext fieldContext)
        {
            this.fieldContext = fieldContext;
        }

        public IFinalizer Catch(Func<object?> valueAccessor)
        {
            return new CatchFinalizer(fieldContext, valueAccessor);
        }
    }
}
