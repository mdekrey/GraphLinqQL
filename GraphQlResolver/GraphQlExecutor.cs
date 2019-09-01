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

        public object Execute(string query)
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
                return Build(builder, ast, def.SelectionSet.Selections);
            });
        }

        private IGraphQlResult<object> Build(IComplexResolverBuilder<object> builder, GraphQLDocument ast, IEnumerable<ASTNode> selections)
        {
            return selections.Aggregate(builder, (b, node) => Build(b, ast, node)).Build();
        }

        private IComplexResolverBuilder<object> Build(IComplexResolverBuilder<object> builder, GraphQLDocument ast, ASTNode node)
        {
            switch (node)
            {
                case GraphQLFieldSelection field:
                    if (field.SelectionSet != null)
                    {
                        return builder.Add(field.Alias?.Value ?? field.Name.Value, b => Build(b.ResolveQuery(field.Name.Value).ResolveComplex(), ast, field.SelectionSet.Selections));
                    }
                    else
                    {
                        return builder.Add(field.Alias?.Value ?? field.Name.Value, field.Name.Value);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        //private Func<IGraphQlResolvable, IGraphQlResult> Resolve(GraphQLDocument ast, GraphQLFieldSelection field)
        //{
        //    return resolvable =>
        //    {
                
        //    };
        //}
    }
}
