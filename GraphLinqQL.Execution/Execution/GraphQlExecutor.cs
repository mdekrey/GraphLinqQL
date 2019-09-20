﻿using GraphQLParser;
using GraphQLParser.AST;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Execution
{
    public class GraphQlExecutor : IGraphQlExecutor
    {
        private readonly IGraphQlServiceProvider serviceProvider;
        private readonly IGraphQlExecutionOptions options;
        private readonly IGraphQlParameterResolverFactory parameterResolverFactory;

        public GraphQlExecutor(IGraphQlServiceProvider serviceProvider, IGraphQlExecutionOptions options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options;
            this.parameterResolverFactory = serviceProvider.GetParameterResolverFactory();
        }

        public object Execute(string query, IDictionary<string, string>? arguments = null)
        {
            var actualArguments = arguments ?? ImmutableDictionary<string, string>.Empty;
            var lexer = new Lexer();
            var parser = new Parser(lexer);
            var ast = parser.Parse(new Source(query));
            var def = ast.Definitions.OfType<GraphQLOperationDefinition>().First();
            if (def == null)
            {
                throw new ArgumentException("Query did not contain a document", nameof(query));
            }

            //var variableDefinitions = def.VariableDefinitions?.ToImmutableDictionary(v => v.Variable.Name.Value, v => GetTypeFromGraphQlType(v.Type))
            //    ?? ImmutableDictionary<string, Type>.Empty;

            var executionResult = Execute(ast, def, actualArguments);
            return executionResult;
        }

        private Type GetTypeFromGraphQlType(GraphQLType arg)
        {
            if (arg is GraphQLNonNullType nonNullType)
            {
                var t = GetTypeFromGraphQlType(nonNullType.Type);
                if (t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return t.GetGenericArguments()[0];
                }
                return t;
            }
            if (arg is GraphQLListType listType)
            {
                var t = GetTypeFromGraphQlType(listType.Type);
                return typeof(IEnumerable<>).MakeGenericType(t);
            }
            if (arg is GraphQLNamedType namedType)
            {
                return options.TypeResolver.Resolve(namedType.Name.Value);
            }
            throw new InvalidOperationException("Variable type was not a list, not-null, or named.");
        }

        private object Execute(GraphQLDocument ast, GraphQLOperationDefinition def, IDictionary<string, string> arguments)
        {
            var operation = def.Operation switch
            {
                OperationType.Query => options.Query,
                OperationType.Mutation => options.Mutation,
                OperationType.Subscription => options.Subscription,
                _ => throw new NotSupportedException()
            } ?? throw new NotSupportedException();

            var context = new GraphQLExecutionContext(ast, arguments);
            return serviceProvider.GraphQlRoot(operation, builder =>
            {
                return Build(builder, def.SelectionSet.Selections, context).Build();
            });
        }

        private IComplexResolverBuilder Build(IComplexResolverBuilder builder, IEnumerable<ASTNode> selections, GraphQLExecutionContext context)
        {
            return selections.Aggregate(builder, (b, node) => Build(b, node, context));
        }

        private IComplexResolverBuilder Build(IComplexResolverBuilder builder, ASTNode node, GraphQLExecutionContext context)
        {
            var resultNode = (node is IHasDirectivesNode directives)
                ? directives.Directives.Aggregate((ASTNode?)node, (node, directive) => node != null ? HandleDirective(directive, node, context) : node)
                : node;
            if (resultNode == null)
            {
                return builder;
            }
            switch (resultNode)
            {
                case GraphQLFieldSelection field:
                    if (field.SelectionSet != null)
                    {
                        return builder.Add(
                            field.Alias?.Value ?? field.Name.Value,
                            b => Build(b.ResolveQuery(field.Name.Value, parameterResolverFactory.FromParameterData(ResolveArguments(field.Arguments, context)))
                                        .ResolveComplex(serviceProvider), field.SelectionSet.Selections, context
                                ).Build()
                        );
                    }
                    else
                    {
                        return builder.Add(field.Alias?.Value ?? field.Name.Value, field.Name.Value, ResolveArguments(field.Arguments, context));
                    }
                case GraphQLFragmentSpread fragmentSpread:
                    return Build(builder,
                        context.Ast.Definitions.OfType<GraphQLFragmentDefinition>().SingleOrDefault(frag => frag.Name.Value == fragmentSpread.Name.Value).SelectionSet.Selections,
                        context);
                case GraphQLInlineFragment inlineFragment:
                    IComplexResolverBuilder DoBuild(IComplexResolverBuilder builder)
                    {
                        return Build(builder,
                            inlineFragment.SelectionSet.Selections,
                            context);
                    }

                    if (inlineFragment.TypeCondition != null)
                    {
                        return builder.IfType(inlineFragment.TypeCondition.Name.Value, DoBuild);
                    }
                    else
                    {
                        return DoBuild(builder);
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private ASTNode? HandleDirective(GraphQLDirective directive, ASTNode node, GraphQLExecutionContext context)
        {
            var arguments = ResolveArguments(directive.Arguments, context);
            var actualDirective = options.Directives.FirstOrDefault(d => d.Name == directive.Name.Value);
            return actualDirective == null
                ? node
                : actualDirective.HandleDirective(node, parameterResolverFactory.FromParameterData(arguments), context);
        }

        private IDictionary<string, string> ResolveArguments(IEnumerable<GraphQLArgument> arguments, GraphQLExecutionContext context)
        {
            return arguments.ToDictionary(arg => arg.Name.Value, arg => ResolveValue(arg.Value, context));
        }

        private string ResolveValue(GraphQLValue value, GraphQLExecutionContext context)
        {
            switch (value)
            {
                case GraphQLScalarValue scalar:
                    // TODO - is there a better way to handle this string?
                    return scalar.Kind == ASTNodeKind.StringValue ? $"\"{scalar.Value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"" : scalar.Value;
                case GraphQLVariable variable:
                    return context.Arguments[variable.Name.Value];
                default:
                    throw new NotSupportedException();
            }
        }

        void IDisposable.Dispose()
        {
            serviceProvider.Dispose();
        }
    }
}
