using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ObjectFieldContext
    {
        private readonly FieldDefinition field;
        private readonly GraphQLGenerationOptions options;

        public ObjectFieldContext(FieldDefinition field, GraphQLGenerationOptions options)
        {
            this.field = field;
            this.options = options;
        }
        public string? Description => field.Description;

        public string Label => field.Name;

        public string Name => CSharpNaming.GetPropertyName(field.Name);

        public string? DeprecationReason
        {
            get
            {
                var obsolete = field.Directives.FirstOrDefault(d => d.Name == "Obsolete");
                if (obsolete != null)
                {
                    var reason = obsolete.Arguments.FirstOrDefault(a => a.Name == "reason")?.Value;
                    if (reason != null)
                    {
                        return options.ValueResolver.Resolve(reason, options);
                    }
                    else
                    {
                        return "";
                    }
                }
                return null;
            }
        }

        public string? TypeName => options.TypeResolver.Resolve(field.TypeNode, options);

        public IEnumerable<InputValueContext> Arguments
        {
            get
            {
                foreach (var arg in field.Arguments)
                {
                    yield return new InputValueContext(arg, options);
                }
            }
        }
    }
}