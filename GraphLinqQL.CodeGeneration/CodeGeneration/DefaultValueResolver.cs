using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    internal class DefaultValueResolver : IValueResolver
    {
        public string Resolve(IValueNode reason, GraphQLGenerationOptions options)
        {
            return "null";
        }


    }
}