using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace GraphQlResolver
{
    public class ComplexResolverBuilder<TContract, TFinal, TModel> : IComplexResolverBuilder<TContract, TFinal>
        where TContract : IGraphQlResolvable, IGraphQlAccepts<TModel>
        where TFinal : IGraphQlResult
    {
        private static readonly System.Reflection.MethodInfo addMethod = typeof(IDictionary<string, object>).GetMethod("Add");
        private readonly TContract contract;
        private readonly Func<Expression<Func<TModel, IDictionary<string, object>>>, TFinal> resolve;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(TContract contract, Func<Expression<Func<TModel, IDictionary<string, object>>>, TFinal> resolve)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(TContract contract, Func<Expression<Func<TModel, IDictionary<string, object>>>, TFinal> resolve, ImmutableDictionary<string, IGraphQlResult> expressions)
            : this(contract, resolve)
        {
            this.expressions = expressions;
        }

        public IComplexResolverBuilder<TContract, TFinal> Add(string property, params object[] parameters) => Add(property, property, parameters);

        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, string property, params object[] parameters)
        {
            var result = contract.ResolveQuery(property, parameters: parameters);
            return new ComplexResolverBuilder<TContract, TFinal, TModel>(contract, resolve, expressions
                .Add(displayName, result));
        }


        public IComplexResolverBuilder<TContract, TFinal> Add(string displayName, Func<TContract, IGraphQlResult> resolve)
        {
            return new ComplexResolverBuilder<TContract, TFinal, TModel>(contract, this.resolve, expressions
                .Add(displayName, resolve(contract)));
        }

        public TFinal Build()
        {
            var modelParameter = Expression.Parameter(typeof(TModel));

            var variable = Expression.Parameter(typeof(Dictionary<string, object>));
            var statements = new List<Expression>()
                    {
                        Expression.Assign(variable, Expression.New(variable.Type)),
                    };

            // TODO - all joins first, create a lookup of values based on join expression, then use that in the next foreach

            foreach (var result in expressions)
            {
                if (result.Value is IGraphQlResultFromInput<TModel> forInput)
                {
                    var inputResolver = forInput.Resolve();
                    var resolveBody = inputResolver.Body.Replace(inputResolver.Parameters[0], with: modelParameter);
                    statements.Add(Expression.Call(variable, addMethod, Expression.Constant(result.Key), resolveBody));
                }
                else
                {
                    throw new InvalidOperationException($"Expected IGraphQlResultFromInput, got {result.Value?.GetType()?.FullName ?? "null"} for {result.Key}");
                }
            }

            statements.Add(Expression.Label(Expression.Label(variable.Type), variable));
            var block = Expression.Block(
                variable.Type,
                new[] { variable },
                statements
            );
            var func = Expression.Lambda<Func<TModel, IDictionary<string, object>>>(block, modelParameter);

            return resolve(func);
        }
    }
}
