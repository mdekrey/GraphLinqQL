using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public interface ITypeResolver
    {
        string Resolve(ITypeNode typeNode, GraphQLGenerationOptions options);
        string ResolveNonNull(ITypeNode typeNode, GraphQLGenerationOptions options);
        bool IsNullable(ITypeNode typeNode, GraphQLGenerationOptions options);
    }
}