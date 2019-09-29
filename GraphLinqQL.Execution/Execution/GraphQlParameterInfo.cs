using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GraphLinqQL.Execution
{
    public class GraphQlParameterInfo : IGraphQlParameterInfo
    {
        private readonly IValueNode valueNode;

        public GraphQlParameterInfo(IValueNode valueNode)
        {
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
            throw new NotImplementedException();
        }
    }
}
