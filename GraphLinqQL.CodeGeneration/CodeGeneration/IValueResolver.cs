using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public interface IValueResolver
    {
        string Resolve(IValueNode value, ITypeNode typeNode, GraphQLGenerationOptions options, Document document);
        string ResolveJson(IValueNode value, ITypeNode typeNode, GraphQLGenerationOptions options, Document document);
    }
}