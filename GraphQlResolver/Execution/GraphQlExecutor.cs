using GraphQLParser;
using GraphQLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQlResolver.Execution
{
    public class GraphQlExecutor<TQuery, TMutation>
        where TQuery : IGraphQlResolvable
        where TMutation : IGraphQlResolvable
    {
        private IServiceProvider serviceProvider;

        public GraphQlExecutor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object Execute(string query, IDictionary<string, object> arguments)
        {
            var lexer = new Lexer();
            var parser = new Parser(lexer);
            var ast = parser.Parse(new Source(query));
            var context = new GraphQLExecutionContext(ast, arguments);
            var def = ast.Definitions.OfType<GraphQLOperationDefinition>().First();
            if (def == null)
            {
                throw new ArgumentException("Query did not contain a document", nameof(query));
            }

            var operation = def.Operation switch
            {
                OperationType.Query => typeof(TQuery),
                OperationType.Mutation => typeof(TMutation),
                OperationType.Subscription => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };

            return serviceProvider.GraphQlRoot(operation, builder =>
            {
                return Build(builder, def.SelectionSet.Selections, context).Build();
            });
        }

        private IComplexResolverBuilder<object> Build(IComplexResolverBuilder<object> builder, IEnumerable<ASTNode> selections, GraphQLExecutionContext context)
        {
            return selections.Aggregate(builder, (b, node) => Build(b, node, context));
        }

        private IComplexResolverBuilder<object> Build(IComplexResolverBuilder<object> builder, ASTNode node, GraphQLExecutionContext context)
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
                            b => Build(b.ResolveQuery(field.Name.Value, ResolveArguments(field.Arguments, context)).ResolveComplex(), field.SelectionSet.Selections, context).Build()
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
                    if (inlineFragment.TypeCondition != null)
                    {
                        // TODO - type conditiono
                    }
                    return Build(builder,
                        inlineFragment.SelectionSet.Selections,
                        context);
                default:
                    throw new NotImplementedException();
            }
        }

        private ASTNode? HandleDirective(GraphQLDirective directive, ASTNode node, GraphQLExecutionContext context)
        {
            var arguments = ResolveArguments(directive.Arguments, context);
            return directive.Name.Value switch
            {
                "include" => Convert.ToBoolean(arguments["if"]) == true ? node : null,
                "skip" => Convert.ToBoolean(arguments["if"]) == false ? node : null,
                _ => node
            };
        }

        private IDictionary<string, object> ResolveArguments(IEnumerable<GraphQLArgument> arguments, GraphQLExecutionContext context)
        {
            return arguments.ToDictionary(arg => arg.Name.Value, arg => ResolveValue(arg.Value, context));
        }

        private object ResolveValue(GraphQLValue value, GraphQLExecutionContext context)
        {
            return value switch
            {
                GraphQLScalarValue scalar => scalar.Value,
                GraphQLVariable variable => context.Arguments[variable.Name.Value],
                _ => throw new NotImplementedException()
            };
        }
    }
}
