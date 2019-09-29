using System;

namespace GraphLinqQL.Ast.Nodes
{
    public class Variable : NodeBase, IValueNode
    {
        public Variable(string name, LocationRange location) : base(location)
        {
            Name = name;
        }

        public override NodeKind Kind => NodeKind.Variable;

        public string Name { get; }

        public object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return converter.VisitVariable(this, converterContext, expectedType, nullable);
        }
    }
}