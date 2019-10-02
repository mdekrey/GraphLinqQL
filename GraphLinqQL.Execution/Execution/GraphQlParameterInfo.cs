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
        private readonly GraphQLExecutionContext context;

        public GraphQlParameterInfo(IValueNode valueNode, GraphQLExecutionContext context)
        {
            this.valueNode = valueNode;
            this.context = context;
        }

        public object? BindTo(Type type)
        {
            return ToObject(type);
        }

        private object? ToObject(Type type)
        {
            return Convert(valueNode, type);
        }

        private object? Convert(IValueNode valueNode, Type type)
        {
            return new ValueConverter().Visit(valueNode, new ValueConverterContext((arg, t) => context.Arguments[arg].BindTo(t)), type);
        }
    }
}
