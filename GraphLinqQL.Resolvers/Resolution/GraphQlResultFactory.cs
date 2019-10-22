using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphLinqQL.Resolution
{
    internal static class GraphQlResultFactory
    {
        public static IGraphQlResultFactory Construct(FieldContext fieldContext, Type modelType)
        {
            return (IGraphQlResultFactory)Activator.CreateInstance(typeof(GraphQlResultFactory<>).MakeGenericType(modelType), fieldContext)!;
        }
    }

}