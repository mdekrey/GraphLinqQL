﻿namespace GraphLinqQL.Ast.Nodes
{
    public class Variable : NodeBase, IValueNode
    {
        public Variable(string name, LocationRange location) : base(location)
        {
            Name = name;
        }

        public override NodeKind Kind => NodeKind.Variable;

        public string Name { get; }
    }
}