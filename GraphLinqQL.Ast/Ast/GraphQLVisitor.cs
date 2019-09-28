﻿using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using GraphLinqQL.Ast.Antlr;
using GraphLinqQL.Ast.Nodes;
using System;
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
            return base.VisitFragmentDefinition(context);
        }

        public override INode VisitTypeSystemDefinition([NotNull] GraphqlParser.TypeSystemDefinitionContext context)
        {
            return base.VisitTypeSystemDefinition(context);
        }

        public override INode VisitOperationType([NotNull] GraphqlParser.OperationTypeContext context)
        {
            throw new InvalidOperationException("Should not reach this code; expected to convert this to an enum");
        }


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
            return new Argument(context.name().GetText(), Visit(context.valueWithVariable()), context.Location());
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

        private void AssertNoException(Antlr4.Runtime.ParserRuleContext context)
        {
            if (context.exception != null)
            {
                throw new GraphqlParseException($"Unable to parse, could not match {context.GetType().Name} at {context.Start.Line}:{context.Start.Column}", context.exception);
            }
        }
    }
}