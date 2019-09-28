using System.Collections.Generic;

namespace GraphLinqQL.Ast.Nodes
{
    public interface IHasDirectives : INode
    {
        IReadOnlyList<Directive> Directives { get; }
    }
}