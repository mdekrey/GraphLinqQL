using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public interface ITypeResolver
    {
        string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options, bool nullable = true, Document? document = null);
        bool IsNullable(ITypeNode typeNode, GraphQLGenerationOptions options);
    }
}