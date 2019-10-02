using GraphLinqQL.Ast;
using GraphLinqQL.Ast.Nodes;
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
        private readonly IAbstractSyntaxTreeGenerator astGenerator;
        private readonly IGraphQlExecutionOptions options;
        private readonly IGraphQlParameterResolverFactory parameterResolverFactory;

        public GraphQlExecutor(IGraphQlServiceProvider serviceProvider, IAbstractSyntaxTreeGenerator astGenerator, IGraphQlExecutionOptions options)
        {
            this.serviceProvider = serviceProvider;
            this.astGenerator = astGenerator;
            this.options = options;
            this.parameterResolverFactory = serviceProvider.GetParameterResolverFactory();
        }

        public object Execute(string query, IDictionary<string, IGraphQlParameterInfo>? arguments = null)
        {
            var actualArguments = arguments ?? ImmutableDictionary<string, IGraphQlParameterInfo>.Empty;
            var ast = astGenerator.ParseDocument(query);
            var def = ast.Children.OfType<OperationDefinition>().First();
            if (def == null)
            {
                throw new ArgumentException("Query did not contain a document", nameof(query));
            }

            //var variableDefinitions = def.VariableDefinitions?.ToImmutableDictionary(v => v.Variable.Name.Value, v => GetTypeFromGraphQlType(v.Type))
            //    ?? ImmutableDictionary<string, Type>.Empty;

            var executionResult = Execute(ast, def, actualArguments);
            return executionResult;
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
                    return options.TypeResolver.Resolve(namedType.Name);
                default:
                    throw new InvalidOperationException("Variable type was not a list, not-null, or named.");
            }
        }

        private object Execute(Document ast, OperationDefinition def, IDictionary<string, IGraphQlParameterInfo> arguments)
        {
            var operation = def.OperationType switch
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
                    var arguments = ResolveArguments(field.Arguments, context);
                    if (field.SelectionSet != null)
                    {
                        return builder.Add(
                            field.Alias ?? field.Name,
                            b => Build(b.ResolveQuery(field.Name, parameterResolverFactory.FromParameterData(arguments))
                                        .ResolveComplex(serviceProvider), field.SelectionSet.Selections, context
                                ).Build()
                        );
                    }
                    else
                    {
                        return builder.Add(field.Alias ?? field.Name, field.Name, arguments);
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
            return actualDirective == null
                ? node
                : actualDirective.HandleDirective(node, parameterResolverFactory.FromParameterData(arguments), context);
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
