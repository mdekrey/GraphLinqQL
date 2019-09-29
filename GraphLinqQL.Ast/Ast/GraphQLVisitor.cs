using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using GraphLinqQL.Ast.Antlr;
using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL.Ast
{
    public class GraphQLVisitor : GraphqlBaseVisitor<INode>
    {
        public override INode VisitDocument([NotNull] GraphqlParser.DocumentContext context)
        {
            
            return new Document(
                context.definition().Select(this.Visit).Cast<IDefinitionNode>(),
                context.Location()
            );
        }

        public override INode VisitOperationDefinition([NotNull] GraphqlParser.OperationDefinitionContext context)
        {
            var op = GetOperationType(context.operationType());
            var name = context.NAME()?.GetText();

            return new OperationDefinition(
                GetOperationType(context.operationType()), 
                context.NAME()?.GetText(), 
                context.variableDefinitions()?.variableDefinition().Select(this.Visit).Cast<VariableDefinition>(), 
                (SelectionSet)Visit(context.selectionSet()), 
                context.Location()
            );
        }

        public override INode VisitFragmentDefinition([NotNull] GraphqlParser.FragmentDefinitionContext context)
        {
            return new FragmentDefinition(
                context.fragmentName().GetText(), 
                (TypeCondition)Visit(context.typeCondition()),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                (SelectionSet)Visit(context.selectionSet()),
                context.Location()
            );
        }

#if DEBUG
        public override INode VisitOperationType([NotNull] GraphqlParser.OperationTypeContext context)
        {
            throw new InvalidOperationException("Should not reach this code; expected to convert this to an enum");
        }
#endif


        public virtual OperationType GetOperationType(GraphqlParser.OperationTypeContext? context)
        {
            if (context?.SUBSCRIPTION() != null)
            {
                return OperationType.Subscription;
            }
            else if (context?.MUTATION() != null)
            {
                return OperationType.Mutation;
            }
            else
            {
                return OperationType.Query;
            }
        }

        public override INode Visit(IParseTree tree)
        {
            if (tree is Antlr4.Runtime.ParserRuleContext ruleContext)
            {
                AssertNoException(ruleContext);
            }

            var result = base.Visit(tree);
#if DEBUG
            if (result == null)
            {
                
                throw new NotImplementedException($"{tree.GetType().Name} should not have returned null, or if it is a union type, see {tree.GetChild(0).GetType().Name}");
            }
#endif
            return result;
        }

        public override INode VisitSelectionSet([NotNull] GraphqlParser.SelectionSetContext context)
        {
            return new SelectionSet(context.selection().Select(Visit).Cast<ISelection>(), context.Location());
        }

        public override INode VisitField([NotNull] GraphqlParser.FieldContext context)
        {
            return new Field(
                context.name().GetText(), 
                context.alias()?.GetText(), 
                context.arguments()?.argument().Select(Visit).Cast<Argument>(), 
                context.directives()?.directive().Select(Visit).Cast<Directive>(), 
                context.selectionSet() == null ? null : (SelectionSet)Visit(context.selectionSet()),
                context.Location()
            );
        }

        public override INode VisitVariableDefinition([NotNull] GraphqlParser.VariableDefinitionContext context)
        {
            return new VariableDefinition((Variable)Visit(context.variable()), (ITypeNode)Visit(context.type()), (IValueNode?)context.defaultValue()?.Accept(this), context.Location());
        }

        public override INode VisitArgument([NotNull] GraphqlParser.ArgumentContext context)
        {
            return new Argument(context.name().GetText(), (IValueNode)Visit(context.valueWithVariable()), context.Location());
        }

        public override INode VisitVariable([NotNull] GraphqlParser.VariableContext context)
        {
            return new Variable(context.name().GetText(), context.Location());
        }

        public override INode VisitTypeName([NotNull] GraphqlParser.TypeNameContext context)
        {
            return new TypeName(context.name().GetText(), context.Location());
        }

        public override INode VisitListType([NotNull] GraphqlParser.ListTypeContext context)
        {
            return new ListType((ITypeNode)Visit(context.type()), context.Location());
        }

        public override INode VisitNonNullType([NotNull] GraphqlParser.NonNullTypeContext context)
        {
            return new NonNullType((ITypeNode)Visit((IParseTree)context.typeName() ?? context.listType()), context.Location());
        }

        public override INode VisitValueWithVariable([NotNull] GraphqlParser.ValueWithVariableContext context)
        {
            var intValue = context.IntValue();
            if (intValue != null)
            {
                return new IntValue(intValue.GetText(), context.Location());
            }
            var floatValue = context.FloatValue();
            if (floatValue != null)
            {
                return new FloatValue(floatValue.GetText(), context.Location());
            }
            var booleanValue = context.BooleanValue();
            if (booleanValue != null)
            {
                return new BooleanValue(booleanValue.GetText(), context.Location());
            }
            var nullValue = context.NullValue();
            if (nullValue != null)
            {
                return new NullValue(context.Location());
            }
            return base.VisitValueWithVariable(context);
        }

        public override INode VisitArrayValue([NotNull] GraphqlParser.ArrayValueContext context)
        {
            return new ArrayValue(context.value().Select(Visit).Cast<IValueNode>(), context.Location());
        }

        public override INode VisitArrayValueWithVariable([NotNull] GraphqlParser.ArrayValueWithVariableContext context)
        {
            return new ArrayValue(context.valueWithVariable().Select(Visit).Cast<IValueNode>(), context.Location());
        }

        public override INode VisitObjectValueWithVariable([NotNull] GraphqlParser.ObjectValueWithVariableContext context)
        {
            return new ObjectValue(context.objectFieldWithVariable().ToDictionary(field => field.name().GetText(), field => (IValueNode)Visit(field.valueWithVariable())), context.Location());
        }

        public override INode VisitObjectValue([NotNull] GraphqlParser.ObjectValueContext context)
        {
            return new ObjectValue(context.objectField().ToDictionary(field => field.name().GetText(), field => (IValueNode)Visit(field.value())), context.Location());
        }

        public override INode VisitStringValue([NotNull] GraphqlParser.StringValueContext context)
        {
            var stringValue = context.StringValue();
            if (stringValue != null)
            {
                return new StringValue(stringValue.GetText(), context.Location());
            }
            else
            {
                return new TripleQuotedStringValue(context.TripleQuotedStringValue().GetText(), context.Location());
            }
        }

        public override INode VisitEnumValue([NotNull] GraphqlParser.EnumValueContext context)
        {
            return new EnumValue(context.GetText(), context.Location());
        }

        public override INode VisitFragmentSpread([NotNull] GraphqlParser.FragmentSpreadContext context)
        {
            return new FragmentSpread(
                context.fragmentName().GetText(),
                context.directives()?.directive().Select(Visit).Cast<Directive>(), 
                context.Location()
            );
        }

        public override INode VisitTypeCondition([NotNull] GraphqlParser.TypeConditionContext context)
        {
            return new TypeCondition((TypeName)Visit(context.typeName()), context.Location());
        }

        public override INode VisitDirective([NotNull] GraphqlParser.DirectiveContext context)
        {
            return new Directive(context.name().GetText(), context.arguments()?.argument().Select(Visit).Cast<Argument>(), context.Location());
        }

        public override INode VisitInlineFragment([NotNull] GraphqlParser.InlineFragmentContext context)
        {
            return new InlineFragment(
                (TypeCondition?)context.typeCondition()?.Accept(this),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                (SelectionSet)Visit(context.selectionSet()),
                context.Location()
            );
        }

        public override INode VisitObjectTypeDefinition([NotNull] GraphqlParser.ObjectTypeDefinitionContext context)
        {
            return new ObjectTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                interfaces: context.implementsInterfaces()?.typeName().Select(Visit).Cast<TypeName>(),
                directives: context.directives()?.directive().Select(Visit).Cast<Directive>(),
                fields: context.fieldsDefinition()?.fieldDefinition().Select(Visit).Cast<FieldDefinition>(),
                location: context.Location()
            );
        }

        public override INode VisitFieldDefinition([NotNull] GraphqlParser.FieldDefinitionContext context)
        {
            return new FieldDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                (ITypeNode)Visit(context.type()),
                context.argumentsDefinition()?.inputValueDefinition().Select(Visit).Cast<InputValueDefinition>(),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.Location()
            );
        }

        public override INode VisitInputValueDefinition([NotNull] GraphqlParser.InputValueDefinitionContext context)
        {
            return new InputValueDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                (ITypeNode)Visit(context.type()),
                (IValueNode?)context.defaultValue()?.Accept(this),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.Location()
            );
        }

        public override INode VisitSchemaDefinition([NotNull] GraphqlParser.SchemaDefinitionContext context)
        {
            return new SchemaDefinition(
                MaybeGetDescription(context.description()),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.operationTypeDefinition().Select(Visit).Cast<OperationTypeDefinition>(),
                context.Location()
            );
        }

        public override INode VisitOperationTypeDefinition([NotNull] GraphqlParser.OperationTypeDefinitionContext context)
        {
            return new OperationTypeDefinition(GetOperationType(context.operationType()), (TypeName)Visit(context.typeName()), context.Location());
        }

        public override INode VisitScalarTypeDefinition([NotNull] GraphqlParser.ScalarTypeDefinitionContext context)
        {
            return new ScalarTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.Location()
            );
        }

        public override INode VisitEnumTypeDefinition([NotNull] GraphqlParser.EnumTypeDefinitionContext context)
        {
            return new EnumTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                context.enumValueDefinitions().enumValueDefinition().Select(Visit).Cast<EnumValueDefinition>(),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.Location()
            );
        }

        public override INode VisitEnumValueDefinition([NotNull] GraphqlParser.EnumValueDefinitionContext context)
        {
            return new EnumValueDefinition(
                (EnumValue)Visit(context.enumValue()),
                MaybeGetDescription(context.description()),
                context.directives()?.directive().Select(Visit).Cast<Directive>(),
                context.Location()
            );
        }

        public override INode VisitInterfaceTypeDefinition([NotNull] GraphqlParser.InterfaceTypeDefinitionContext context)
        {
            return new InterfaceTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                directives: context.directives()?.directive().Select(Visit).Cast<Directive>(),
                fields: context.fieldsDefinition()?.fieldDefinition().Select(Visit).Cast<FieldDefinition>(),
                location: context.Location()
            );
        }

        public override INode VisitUnionTypeDefinition([NotNull] GraphqlParser.UnionTypeDefinitionContext context)
        {
            return new UnionTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                directives: context.directives()?.directive().Select(Visit).Cast<Directive>(),
                unionMembers: GetUnionMembers(context.unionMembership()),
                location: context.Location()
            );
        }

        public override INode VisitInputObjectTypeDefinition([NotNull] GraphqlParser.InputObjectTypeDefinitionContext context)
        {
            return new InputObjectTypeDefinition(
                context.name().GetText(),
                MaybeGetDescription(context.description()),
                directives: context.directives()?.directive().Select(Visit).Cast<Directive>(),
                fields: context.inputObjectValueDefinitions()?.inputValueDefinition().Select(Visit).Cast<InputValueDefinition>(),
                location: context.Location()
            );
        }

        private IEnumerable<TypeName> GetUnionMembers(GraphqlParser.UnionMembershipContext unionMembershipContext)
        {
            var current = unionMembershipContext.unionMembers();
            while (current != null)
            {
                yield return (TypeName)Visit(current.typeName());
                current = current.unionMembers();
            }
        }

        private void AssertNoException(Antlr4.Runtime.ParserRuleContext context)
        {
            if (context.exception != null)
            {
                throw new GraphqlParseException($"Unable to parse, could not match {context.GetType().Name} at {context.Start.Line}:{context.Start.Column}", context.exception);
            }
        }

        private string? MaybeGetDescription(GraphqlParser.DescriptionContext? description)
        {
            return ((IStringValue?)description?.Accept(this))?.Text;
        }

    }
}