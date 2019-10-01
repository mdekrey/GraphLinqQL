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

        public string TypeName => options.TypeResolver.Resolve(arg.TypeNode, options);
        public string TypeNameNonNull => options.TypeResolver.Resolve(arg.TypeNode, options);

        public string FieldName => CSharpNaming.GetFieldName(arg.Name);

        public string DefaultValue => arg.DefaultValue != null ? options.ValueResolver.Resolve(arg.DefaultValue, options) : "null";

        public string GetParameterWithDefault()
        {
            var fieldName = FieldName;
            var inputTypeName = TypeName;
            var nullable = options.TypeResolver.IsNullable(arg.TypeNode, options);

            var getValue = nullable
                ? $"(parameters.HasParameter(\"{fieldName}\") ? parameters.GetParameter<{inputTypeName}>(\"{fieldName}\") : null)"
                : $"parameters.GetParameter<{inputTypeName}>(\"{fieldName}\")";
            var defaultValueExpression = arg.DefaultValue != null ? $" ?? {options.ValueResolver.Resolve(arg.DefaultValue, options)}" : "";
            return $"{fieldName}: {getValue}{defaultValueExpression}";
        }
    }
}