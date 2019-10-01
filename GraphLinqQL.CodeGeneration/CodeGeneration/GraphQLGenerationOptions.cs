using GraphLinqQL.Ast.Nodes;
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


        public IList<ScalarTypeDefinition> ScalarTypes { get; set; } = new List<ScalarTypeDefinition>
        {
            { new ScalarTypeDefinition("ID", @"The `ID` scalar type represents a unique identifier, often used to refetch an object or as key for a cache. The ID type appears in a JSON response as a String; however, it is not intended to be human-readable. When expected as an input type, any string (such as `""4""`) or integer (such as `4`) input value will be accepted as an ID.", null, new LocationRange()) },
            { new ScalarTypeDefinition("Int", "The `Int` scalar type represents non-fractional signed whole numeric values. Int can represent values between -(2^31) and 2^31 - 1.", null, new LocationRange()) },
            { new ScalarTypeDefinition("Float", "The `Float` scalar type represents signed double-precision fractional values as specified by [IEEE 754](https://en.wikipedia.org/wiki/IEEE_floating_point).", null, new LocationRange()) },
            { new ScalarTypeDefinition("String", "The `String` scalar type represents textual data, represented as UTF-8 character sequences. The String type is most often used by GraphQL to represent free-form human-readable text.", null, new LocationRange()) },
            { new ScalarTypeDefinition("Boolean", "The `Boolean` scalar type represents `true` or `false`.", null, new LocationRange()) }
        };

        public IDictionary<string, TypeReference> ScalarTypeMappings { get; set; } = new Dictionary<string, TypeReference>
        {
            { "ID", new TypeReference(csharpNullable: true, csharpType: "string") },
            { "Int", new TypeReference(csharpNullable: false, csharpType: "int") },
            { "Float", new TypeReference(csharpNullable: false, csharpType: "double") },
            { "String", new TypeReference(csharpNullable: true, csharpType: "string") },
            { "Boolean", new TypeReference(csharpNullable: false, csharpType: "bool") }
        };

        public ITypeResolver TypeResolver { get; set; } = new DefaultTypeResolver();
        public IValueResolver ValueResolver { get; set; } = new DefaultValueResolver();
    
    }
}
