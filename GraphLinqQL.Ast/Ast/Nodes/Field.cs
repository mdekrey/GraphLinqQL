using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class Field : NodeBase, IHasDirectives, ISelection
    {
        public Field(string name, string? alias, IEnumerable<Argument>? arguments, IEnumerable<Directive>? directives, SelectionSet? selectionSet, LocationRange location)
            : base(location)
        {
            Name = name;
            Alias = alias;
            SelectionSet = selectionSet;
            Arguments = arguments?.ToArray() ?? EmptyArrayHelper.Empty<Argument>();
            Directives = directives?.ToArray() ?? EmptyArrayHelper.Empty<Directive>();
        }

        public override NodeKind Kind => NodeKind.Field;

        public string Name { get; }
        public string? Alias { get; }
        public SelectionSet? SelectionSet { get; }
        public IReadOnlyList<Argument> Arguments { get; }
        public IReadOnlyList<Directive> Directives { get; }
    }
}
