using System;
using System.Collections.Immutable;

namespace GraphQlResolver
{
    public class ComplexResolverBuilder<TContract, TFinal, TModel> : IComplexResolverBuilder<TContract, TFinal>
        where TContract : IGraphQlResolvable, IGraphQlAccepts<TModel>
        where TFinal : IGraphQlResult
    {
        private readonly TContract contract;
        private readonly Func<ImmutableDictionary<string, IGraphQlResult>, TFinal> resolve;
        private readonly ImmutableDictionary<string, IGraphQlResult> expressions;

        public ComplexResolverBuilder(TContract contract, Func<ImmutableDictionary<string, IGraphQlResult>, TFinal> resolve)
        {
            this.contract = contract;
            this.resolve = resolve;
            this.expressions = ImmutableDictionary<string, IGraphQlResult>.Empty;
        }

        protected ComplexResolverBuilder(TContract contract, Func<ImmutableDictionary<string, IGraphQlResult>, TFinal> resolve, ImmutableDictionary<string, IGraphQlResult> expressions)
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
            return resolve(expressions);
        }
    }
}
