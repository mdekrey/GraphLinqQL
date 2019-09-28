using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.Ast
{
    public interface IAbstractSyntaxTreeGenerator
    {
        Document ParseDocument(string text);
    }
}