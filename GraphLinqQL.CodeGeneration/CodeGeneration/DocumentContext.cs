using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphLinqQL.Ast.Nodes;
using static GraphLinqQL.CodeGeneration.ErrorCodes;

namespace GraphLinqQL.CodeGeneration
{
    class DocumentContext
    {
        private readonly Document document;
        private readonly GraphQLGenerationOptions options;

        public DocumentContext(Document document, string filename, GraphQLGenerationOptions options)
        {
            this.document = document;
            UsingStatements = options.UsingStatements.ToArray();
            FileName = filename;
            this.options = options;
        }

        public IReadOnlyList<string> UsingStatements { get; }

        public bool Nullability => options.ShowNullabilityIndicators();

        public string Namespace => options.Namespace;
        public string FileName { get; }

        public IEnumerable<ScalarTypeContext> ScalarTypes
        {
            get
            {
                foreach (var scalarType in options.ScalarTypes)
                {
                    yield return new ScalarTypeContext(scalarType, options);
                }
            }
        }

        public IEnumerable<InputObjectTypeContext> InputObjectTypes
        {
            get
            {
                foreach (var declaration in document.Children.OfType<InputObjectTypeDefinition>())
                {
                    yield return new InputObjectTypeContext(declaration, options, document);
                }
                yield break;
            }
        }

        public IEnumerable<ITypeDeclaration> GetTypeDeclarations()
        {
            return document.Children.Concat(options.ScalarTypes).Select(def => def switch
            {
                ObjectTypeDefinition objectTypeDefinition => new ObjectTypeContext(objectTypeDefinition, options, document) as ITypeDeclaration,
                InputObjectTypeDefinition inputObjectTypeDefinition => new InputObjectTypeContext(inputObjectTypeDefinition, options, document) as ITypeDeclaration,
                InterfaceTypeDefinition interfaceTypeDefinition => new InterfaceTypeContext(interfaceTypeDefinition, options, document) as ITypeDeclaration,
                EnumTypeDefinition enumTypeDefinition => new EnumTypeContext(enumTypeDefinition, options, document) as ITypeDeclaration,
                UnionTypeDefinition unionTypeDefinition => new UnionTypeContext(unionTypeDefinition, options) as ITypeDeclaration,
                ScalarTypeDefinition scalarTypeDefinition => new ScalarTypeContext(scalarTypeDefinition, options) as ITypeDeclaration,
                SchemaDefinition _ => null,
                _ => throw new InvalidOperationException("Unhandled definition type " + def.Kind.ToString("g"))
            })
                .Where(v => v != null)!;
        }

        public IEnumerable<DirectiveContext> Directives
        {
            get
            {
                yield return new DirectiveContext(new DirectiveDefinition("skip", @"Directs the executor to skip this field or fragment when the `if` argument is true.",
                    new[] { "FIELD", "FRAGMENT_SPREAD", "INLINE_FRAGMENT" },
                    new[] { new InputValueDefinition("if", "Skipped when true.", new NonNullType(new TypeName("Boolean", new LocationRange()), new LocationRange()), null, null, new LocationRange()) },
                    new LocationRange()), options, document);
                yield return new DirectiveContext(new DirectiveDefinition("include", @"Directs the executor to include this field or fragment only when the `if` argument is true.",
                    new[] { "FIELD", "FRAGMENT_SPREAD", "INLINE_FRAGMENT" },
                    new[] { new InputValueDefinition("if", @"Included when true.", new NonNullType(new TypeName("Boolean", new LocationRange()), new LocationRange()), null, null, new LocationRange()) },
                    new LocationRange()), options, document);
                //yield return new DirectiveContext(new DirectiveDefinition("deprecated", @"Marks an element of a GraphQL schema as no longer supported.",
                //    new[] { "FIELD_DEFINITION", "ENUM_VALUE" },
                //    new[] {
                //        new InputValueDefinition(
                //            "reason",
                //            @"Explains why this element was deprecated, usually also including a suggestion for how to access supported similar data. Formatted using the Markdown syntax (as specified by [CommonMark](https://commonmark.org/).",
                //            new TypeName("String", new LocationRange()),
                //            new StringValue(@"""No longer supported""", new LocationRange()),
                //            null,
                //            new LocationRange())
                //    },
                //    new LocationRange()), options, document);

                foreach (var directive in document.Children.OfType<DirectiveDefinition>())
                {
                    yield return new DirectiveContext(directive, options, document);
                }

            }
        }

        public IEnumerable<CompilerError> CompilerErrors
        {
            get
            {
                var unreferencedScalarTypes = document.Children.OfType<ScalarTypeDefinition>()
                    .Where(def => !options.ScalarTypes.Select(scalarType => scalarType.Name).Contains(def.Name))
                    .ToArray();
                foreach (var unreferencedScalarType in unreferencedScalarTypes)
                {
                    yield return new CompilerError(FileName, unreferencedScalarType.Location.Start.Line, unreferencedScalarType.Location.Start.Column, UnknownScalarTypeErrorCode, UnknownScalarTypeMessage(unreferencedScalarType.Name))
                    {
                        IsWarning = true
                    };
                }
            }
        }

        public string Introspection(TextWriter writer, string indentation)
        {
            Templates.IntrospectionNamespaceGenerator.RenderIntrospection(this, writer, indentation);
            return "";
        }

        public string QueryTypeName => ToTypeof(FindOperation(OperationType.Query)?.TypeName);
        public string MutationTypeName => ToTypeof(FindOperation(OperationType.Mutation)?.TypeName);
        public string SubscriptionTypeName => ToTypeof(FindOperation(OperationType.Subscription)?.TypeName);

        private OperationTypeDefinition? FindOperation(OperationType operationType) =>
            document.Children.OfType<SchemaDefinition>().FirstOrDefault()?.OperationTypes?.FirstOrDefault(def => def.OperationType == operationType);

        private static string ToTypeof(TypeName? typeName) => typeName switch
        {
            null => "null",
            _ => $"typeof({CSharpNaming.GetTypeName(typeName.Name)})"
        };

        private static string ToTypeName(TypeReference value)
        {
            return $"{value.CsharpType}{(value.CsharpNullable ? "" : "?")}";
        }
    }
}
