using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ObjectTypeContext : ITypeDeclaration
    {
        private readonly ObjectTypeDefinition declaration;
        private readonly GraphQLGenerationOptions options;

        public ObjectTypeContext(ObjectTypeDefinition declaration, GraphQLGenerationOptions options)
        {
            this.declaration = declaration;
            this.options = options;
        }

        public string Label => declaration.Name;
        public string TypeName => CSharpNaming.GetTypeName(declaration.Name);

        public string? Description => declaration.Description;

        public void Write(TextWriter writer, string indentation)
        {
            Templates.ObjectTypeGenerator.RenderObjectType(this, writer, indentation);
        }

        public IEnumerable<string> ImplementedInterfaces
        {
            get
            {
                foreach (var iface in declaration.Interfaces)
                {
                    yield return CSharpNaming.GetTypeName(iface.Name);
                }
            }
        }

        public IEnumerable<ObjectFieldContext> Fields
        {
            get
            {
                foreach (var field in declaration.Fields)
                {
                    yield return new ObjectFieldContext(field, options);
                }
            }
        }

        public IEnumerable<string> ValidTypes
        {
            get
            {
                yield return TypeName;
                foreach (var iface in ImplementedInterfaces)
                {
                    yield return iface;
                }
            }
        }
    }
}