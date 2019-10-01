using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public interface IValueResolver
    {
        string Resolve(IValueNode reason, GraphQLGenerationOptions options);
    }
}