using GraphQLParser;
using GraphQLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQlResolver
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
            var def = ast.Definitions.First() as GraphQLOperationDefinition;
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
                return Build(builder, ast, def.SelectionSet.Selections, arguments);
            });
        }

        private IGraphQlResult<object> Build(IComplexResolverBuilder<object> builder, GraphQLDocument ast, IEnumerable<ASTNode> selections, IDictionary<string, object> arguments)
        {
            return selections.Aggregate(builder, (b, node) => Build(b, ast, node, arguments)).Build();
        }

        private IComplexResolverBuilder<object> Build(IComplexResolverBuilder<object> builder, GraphQLDocument ast, ASTNode node, IDictionary<string, object> arguments)
        {
            switch (node)
            {
                case GraphQLFieldSelection field:
                    if (field.SelectionSet != null)
                    {
                        return builder.Add(
                            field.Alias?.Value ?? field.Name.Value, 
                            b => Build(b.ResolveQuery(field.Name.Value, ResolveArguments(field.Arguments, ast, arguments)).ResolveComplex(), ast, field.SelectionSet.Selections, arguments)
                        );
                    }
                    else
                    {
                        return builder.Add(field.Alias?.Value ?? field.Name.Value, field.Name.Value, ResolveArguments(field.Arguments, ast, arguments));
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private IDictionary<string, object> ResolveArguments(IEnumerable<GraphQLArgument> arguments, GraphQLDocument ast, IDictionary<string, object> queryArguments)
        {
            return arguments.ToDictionary(arg => arg.Name.Value, arg => ResolveValue(arg.Value, ast, queryArguments));
        }

        private object ResolveValue(GraphQLValue value, GraphQLDocument ast, IDictionary<string, object> queryArguments)
        {
            return value switch
            {
                GraphQLScalarValue scalar => scalar.Value,
                GraphQLVariable variable => queryArguments[variable.Name.Value],
                _ => throw new NotImplementedException()
            };
        }
    }
}
