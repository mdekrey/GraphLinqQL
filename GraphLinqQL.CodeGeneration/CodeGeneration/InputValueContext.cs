using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InputValueContext
    {
        private readonly InputValueDefinition arg;
        private readonly GraphQLGenerationOptions options;
        private readonly Document document;

        public InputValueContext(InputValueDefinition arg, GraphQLGenerationOptions options, Document document, string propertyName)
        {
            this.arg = arg;
            this.options = options;
            this.document = document;
            this.PropertyName = propertyName;
        }

        public string Label => arg.Name;
        public string? Description => arg.Description;

        public string TypeName => options.Resolve(arg.TypeNode, document);
        public string TypeNameNonNull => options.Resolve(arg.TypeNode, document, nullable: false);

        public string FieldName => CSharpNaming.GetFieldName(arg.Name);
        public string PropertyName { get; }

        public string? DefaultValue => arg.DefaultValue != null ? options.Resolve(arg.DefaultValue, arg.TypeNode, document) : null;
        public string JsonDefaultValue => arg.DefaultValue != null ? options.ResolveJson(arg.DefaultValue, arg.TypeNode, document) : "null";

        public string IntrospectionType => options.ResolveIntrospection(arg.TypeNode);

        public string GetParameterWithDefault()
        {
            var fieldName = FieldName;
            var inputTypeName = TypeName;
            var nullable = options.TypeResolver.IsNullable(arg.TypeNode, options, document);

            var getValue = nullable
                ? $"(parameters.HasParameter(\"{fieldName}\") ? parameters.GetParameter<{inputTypeName}>(\"{fieldName}\") : null)"
                : $"parameters.GetParameter<{inputTypeName}>(\"{fieldName}\")";
            var defaultValueExpression = arg.DefaultValue != null ? $" ?? {options.Resolve(arg.DefaultValue, arg.TypeNode, document)}" : "";
            return $"{fieldName}: {getValue}{defaultValueExpression}";
        }
    }
}