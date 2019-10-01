﻿using GraphLinqQL.Ast.Nodes;
using System.Collections.Generic;
using System.IO;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct ObjectTypeContext : ITypeDeclaration
    {
        private readonly ObjectTypeDefinition declaration;
        private readonly GraphQLGenerationOptions options;
        private readonly Document document;

        public ObjectTypeContext(ObjectTypeDefinition declaration, GraphQLGenerationOptions options, Document document)
        {
            this.declaration = declaration;
            this.options = options;
            this.document = document;
        }

        public GraphQLGenerationOptions Options => options;

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
                    yield return new ObjectFieldContext(field, options, document);
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

        public string TypeKind => "Object";

        public IEnumerable<string>? PossibleTypes => null;

        public IEnumerable<EnumValueContext>? EnumValues => null;

        public IEnumerable<InputValueContext>? InputFields => null;
    }
}