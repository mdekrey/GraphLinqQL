using System;

namespace GraphLinqQL.CodeGeneration
{
    public class TypeReference
    {
        public TypeReference(string csharpType, bool csharpNullable)
        {
            CsharpNullable = csharpNullable;
            CsharpType = csharpType;
        }

        public bool CsharpNullable { get; }
        public string CsharpType { get; }
    }
}