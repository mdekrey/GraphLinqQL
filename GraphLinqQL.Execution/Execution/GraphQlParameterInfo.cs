using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Execution
{
    public class GraphQlParameterInfo : IGraphQlParameterInfo
    {
        private readonly IValueNode valueNode;

        public GraphQlParameterInfo(IValueNode valueNode)
        {
            if (valueNode is Variable)
            {
                throw new ArgumentException("Cannot be a variable itself.", nameof(valueNode));
            }
            this.valueNode = valueNode;
        }

        public T BindTo<T>(IGraphQlParameterResolver variableResolver)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return (T)ToObject(typeof(T), variableResolver);
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        private object? ToObject(Type type, IGraphQlParameterResolver variableResolver)
        {
            return Convert(valueNode, type, variableResolver);
        }

        private object? Convert(IValueNode valueNode, Type type, IGraphQlParameterResolver variableResolver)
        {
            // TODO - variables, nullability
            return new ValueConverter().Visit(valueNode, new ValueConverterContext(variableResolver.GetParameter<object>), type);
        }
    }
}
