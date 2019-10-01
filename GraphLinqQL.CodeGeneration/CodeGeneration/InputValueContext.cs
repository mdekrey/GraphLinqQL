using GraphLinqQL.Ast.Nodes;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct InputValueContext
    {
        private readonly InputValueDefinition arg;
        private readonly GraphQLGenerationOptions options;

        public InputValueContext(InputValueDefinition arg, GraphQLGenerationOptions options)
        {
            this.arg = arg;
            this.options = options;
        }

        public string Label => arg.Name;
        public string? Description => arg.Description;

        public string TypeName => options.Resolve(arg.TypeNode);
        public string TypeNameNonNull => options.Resolve(arg.TypeNode, nullable: false);

        public string FieldName => CSharpNaming.GetFieldName(arg.Name);

        public string DefaultValue => arg.DefaultValue != null ? options.Resolve(arg.DefaultValue, arg.TypeNode) : "null";

        public string IntrospectionType => options.ResolveIntrospection(arg.TypeNode);

        public string GetParameterWithDefault()
        {
            var fieldName = FieldName;
            var inputTypeName = TypeName;
            var nullable = options.TypeResolver.IsNullable(arg.TypeNode, options);

            var getValue = nullable
                ? $"(parameters.HasParameter(\"{fieldName}\") ? parameters.GetParameter<{inputTypeName}>(\"{fieldName}\") : null)"
                : $"parameters.GetParameter<{inputTypeName}>(\"{fieldName}\")";
            var defaultValueExpression = arg.DefaultValue != null ? $" ?? {options.Resolve(arg.DefaultValue, arg.TypeNode)}" : "";
            return $"{fieldName}: {getValue}{defaultValueExpression}";
        }
    }
}