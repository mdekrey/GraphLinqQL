using GraphLinqQL.Ast;
using GraphLinqQL.Ast.Nodes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GraphLinqQL.Execution
{

    public class GraphQlExecutor : IGraphQlExecutor
    {
        private readonly IGraphQlServiceProvider serviceProvider;
        private readonly IAbstractSyntaxTreeGenerator astGenerator;
        private readonly IGraphQlExecutionOptions options;
        private readonly ILogger<GraphQlExecutor> logger;

        public GraphQlExecutor(IGraphQlServiceProvider serviceProvider, IAbstractSyntaxTreeGenerator astGenerator, IGraphQlExecutionOptions options, ILoggerFactory loggerFactory)
        {
            this.serviceProvider = serviceProvider;
            this.astGenerator = astGenerator;
            this.options = options;
            this.logger = loggerFactory.CreateLogger<GraphLinqQL.Execution.GraphQlExecutor>();
        }

        public ExecutionResult Execute(string query, IDictionary<string, IGraphQlParameterInfo>? arguments = null)
        {
            IGraphQlResult result;
            try
            {
                var actualArguments = arguments ?? ImmutableDictionary<string, IGraphQlParameterInfo>.Empty;
                var ast = astGenerator.ParseDocument(query);
                var def = ast.Children.OfType<OperationDefinition>().FirstOrDefault();
                if (def == null)
                {
                    throw new ArgumentException("Query did not contain a document", nameof(query)).AddGraphQlError(WellKnownErrorCodes.NoOperation, ast.Location.ToQueryLocations());
                }

                result = Resolve(ast, def, actualArguments);
            }
            catch (Exception ex)
            {
                ex.ConvertAstExceptions();
                if (ex.HasGraphQlErrors(out var errors))
                {
                    logger.LogWarning(new EventId(10001, "GraphQLParse"), ex, "Caught exception with GraphQL errors during parse");
                    return new ExecutionResult(true, null, errors);
                }
                else
                {
                    throw;
                }
            }
            try
            {
                return result.InvokeResult(new GraphQlRoot());
            }
            catch (Exception ex)
            {
                if (ex.HasGraphQlErrors(out var errors))
                {
                    logger.LogWarning(new EventId(10002, "GraphQLExecution"), ex, "Caught exception with GraphQL errors during execution");
                    return new ExecutionResult(false, new { }, errors);
                }
                else
                {
                    throw;
                }
            }
        }

        private Type GetTypeFromGraphQlType(ITypeNode arg)
        {
            switch (arg)
            {
                case NonNullType nonNullNode:
                    var nonNullType = GetTypeFromGraphQlType(nonNullNode.BaseType);
                    if (nonNullType.IsConstructedGenericType && nonNullType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return nonNullType.GetGenericArguments()[0];
                    }
                    return nonNullType;
                case ListType listNode:
                    var listType = GetTypeFromGraphQlType(listNode.ElementType);
                    return typeof(IEnumerable<>).MakeGenericType(listType);
                case TypeName namedType:
                    return options.TypeResolver.ResolveForInput(namedType.Name);
                default:
                    throw new InvalidOperationException("Variable type was not a list, not-null, or named.");
            }
        }

        private IGraphQlResult Resolve(Document ast, OperationDefinition def, IDictionary<string, IGraphQlParameterInfo> arguments)
        {
            var operation = def.OperationType switch
            {
                OperationType.Query => options.Query,
                OperationType.Mutation => options.Mutation,
                OperationType.Subscription => options.Subscription,
                _ => throw new NotSupportedException()
            } ?? throw new NotSupportedException();

            var actualArguments = def.Variables.ToDictionary(variable => variable.Variable.Name, variable => arguments.ContainsKey(variable.Variable.Name) ? arguments[variable.Variable.Name] : new GraphQlParameterInfo(variable.DefaultValue!, null!));
            var context = new GraphQLExecutionContext(ast, actualArguments);
            var result = serviceProvider.GetResult<GraphQlRoot>(operation, builder =>
            {
                return Build(builder, def.SelectionSet.Selections, context).Build();
            });
            return result;
        }

        private IComplexResolverBuilder Build(IComplexResolverBuilder builder, IEnumerable<ISelection> selections, GraphQLExecutionContext context)
        {
            return selections.Aggregate(builder, (b, node) => Build(b, node, context));
        }

        private IComplexResolverBuilder Build(IComplexResolverBuilder builder, ISelection node, GraphQLExecutionContext context)
        {
            var resultNode = TryGetDirectives(node, out var directives)
                ? directives.Aggregate((ISelection?)node, (node, directive) => node != null ? HandleDirective(directive, node, context) : node)
                : node;
            if (resultNode == null)
            {
                return builder;
            }
            switch (resultNode)
            {
                case Field field:
                    var queryContext = new FieldContext(field.Name, node.Location.ToQueryLocations());
                    var arguments = ResolveArguments(field.Arguments, context);
                    if (field.SelectionSet != null)
                    {
                        return builder.Add(
                            field.Alias ?? field.Name,
                            queryContext,
                            b =>
                            {
                                var result = b.ResolveQuery(field.Name, queryContext, new BasicParameterResolver(arguments));
                                if (!(result is IGraphQlObjectResult objectResult))
                                {
                                    throw new InvalidOperationException("Result does not have a contract assigned to resolve complex objects").AddGraphQlError(WellKnownErrorCodes.NoSubselectionAllowed, queryContext.Locations, new { fieldName = queryContext.Name, type = b.GraphQlTypeName });
                                }
                                return Build(objectResult.ResolveComplex(serviceProvider, queryContext), field.SelectionSet.Selections, context
                                    ).Build();
                            }
                        );
                    }
                    else
                    {
                        return builder.Add(field.Alias ?? field.Name, field.Name, queryContext, new BasicParameterResolver(arguments));
                    }
                case FragmentSpread fragmentSpread:
                    return Build(builder,
                        context.Ast.Children.OfType<FragmentDefinition>().SingleOrDefault(frag => frag.Name == fragmentSpread.FragmentName).SelectionSet.Selections,
                        context);
                case InlineFragment inlineFragment:
                    IComplexResolverBuilder DoBuild(IComplexResolverBuilder builder)
                    {
                        return Build(builder,
                            inlineFragment.SelectionSet.Selections,
                            context);
                    }

                    if (inlineFragment.TypeCondition != null)
                    {
                        return builder.IfType(inlineFragment.TypeCondition.TypeName.Name, DoBuild);
                    }
                    else
                    {
                        return DoBuild(builder);
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool TryGetDirectives(INode node, out IEnumerable<Directive> directives)
        {
            directives = node is IHasDirectives hasDirectives ? hasDirectives.Directives : null!;
            return directives != null;
        }

        private TNode? HandleDirective<TNode>(Directive directive, TNode node, GraphQLExecutionContext context)
            where TNode : class, INode
        {
            var arguments = ResolveArguments(directive.Arguments, context);
            var actualDirective = options.Directives.FirstOrDefault(d => d.Name == directive.Name);
            var fieldContext = new FieldContext(directive.Name, node.Location.ToQueryLocations());
            return actualDirective == null
                ? node
                : actualDirective.HandleDirective(node, new BasicParameterResolver(arguments), fieldContext, context);
        }

        private static IDictionary<string, IGraphQlParameterInfo> ResolveArguments(IReadOnlyList<Argument> arguments, GraphQLExecutionContext context)
        {
            return arguments.ToDictionary(arg => arg.Name, arg => (IGraphQlParameterInfo)new GraphQlParameterInfo(arg.Value, context));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serviceProvider.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
