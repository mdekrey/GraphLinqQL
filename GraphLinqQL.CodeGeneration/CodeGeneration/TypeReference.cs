using System;

namespace GraphLinqQL.CodeGeneration
{
    public class TypeReference
    {
        public TypeReference(string graphQLType, string csharpType, bool csharpNullable)
        {
            CsharpNullable = csharpNullable;
            CsharpType = csharpType;

            GraphQLType = graphQLType;
        }

        public bool CsharpNullable { get; }
        public string CsharpType { get; }
        public string GraphQLType { get; }
    }
}