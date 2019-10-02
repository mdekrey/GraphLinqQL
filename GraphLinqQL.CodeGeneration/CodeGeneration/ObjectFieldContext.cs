using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ObjectFieldContext
    {
        private readonly FieldDefinition field;
        private readonly GraphQLGenerationOptions options;
        private readonly Document document;

        public ObjectFieldContext(FieldDefinition field, GraphQLGenerationOptions options, Document document)
        {
            this.field = field;
            this.options = options;
            this.document = document;
        }
        public string? Description => field.Description;

        public string Label => field.Name;

        public string Name => CSharpNaming.GetPropertyName(field.Name);

        public bool IsDeprecated => field.Directives.FindObsoleteDirective() != null;
        public string? DeprecationReason => field.Directives.FindObsoleteDirective()?.ObsoleteReason(options, document);

        public string? TypeName => options.Resolve(field.TypeNode, document: document);

        public string IntrospectionType => options.ResolveIntrospection(field.TypeNode);

        public IEnumerable<InputValueContext> Arguments
        {
            get
            {
                foreach (var arg in field.Arguments)
                {
                    yield return new InputValueContext(arg, options, document);
                }
            }
        }
    }
}