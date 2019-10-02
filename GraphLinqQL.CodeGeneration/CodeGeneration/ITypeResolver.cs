using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public interface ITypeResolver
    {
        string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options, Document document, bool nullable = true);
        bool IsNullable(ITypeNode typeNode, GraphQLGenerationOptions options, Document document);
    }
}