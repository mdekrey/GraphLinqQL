using System;
using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InputObjectFieldContext
    {
        private readonly InputValueDefinition field;
        private readonly GraphQLGenerationOptions options;

        public InputObjectFieldContext(InputValueDefinition field, GraphQLGenerationOptions options)
        {
            this.field = field;
            this.options = options;
        }

        public string Label => field.Name;
        public string? Description => field.Description;

        public string? TypeName => options.Resolve(field.TypeNode);
        public string PropertyName => CSharpNaming.GetPropertyName(field.Name);

        public string? DefaultValue => field.DefaultValue == null ? "null" : options.Resolve(field.DefaultValue, field.TypeNode);

        public string IntrospectionType => options.ResolveIntrospection(field.TypeNode);

    }
}