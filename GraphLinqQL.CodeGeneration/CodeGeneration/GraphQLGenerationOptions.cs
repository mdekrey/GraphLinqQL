using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
// Allowing people to override the list in Options is good practice
#pragma warning disable CA2227 // Collection properties should be read only

namespace GraphLinqQL.CodeGeneration
{
    public class GraphQLGenerationOptions
    {
        public string Namespace { get; set; } = "Unspecified";

        public int LanguageVersion { get; set; } = 8;

        public IList<string> UsingStatements { get; set; } = new List<string>
        {
            "GraphLinqQL",
            "GraphLinqQL.Introspection",
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.Collections.Immutable",
            "System.Linq"
        };


        public IList<TypeReference> ScalarTypes { get; set; } = new List<TypeReference>
        {
            { new TypeReference(graphQLType: "Int", csharpNullable: false, csharpType: "int") },
            { new TypeReference(graphQLType: "Float", csharpNullable: false, csharpType: "double") },
            { new TypeReference(graphQLType: "String", csharpNullable: true, csharpType: "string") },
            { new TypeReference(graphQLType: "Boolean", csharpNullable: false, csharpType: "bool") }
        };

        public ITypeResolver TypeResolver { get; set; } = new DefaultTypeResolver();
        public IValueResolver ValueResolver { get; set; } = new DefaultValueResolver();
    
    }
}
