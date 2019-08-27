using GraphQlSchema;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace GraphQlResolver
{
    public static class Resolve
    {
        public static IQueryable<T> Query<T>() // TODO - this is a temp function for tests
        {
            return new Query<T>(new GraphQlQueryProvider());
        }


        public static IGraphQlResolver<T> Root<T>()
            where T : IGraphQlAccepts<GraphQlRoot>, IGraphQlResolvable, new() // TODO - don't require this new()
        {
            return new GraphQlResolver<T>(new Query<GraphQlRoot>(new GraphQlQueryProvider()));
        }

        public static IQueryable<TResult> FromMany<T, TResult>()
            where T : IResolutionFactory<IReadOnlyList<TResult>>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<TResult> AsGraphQl<T, TResult>(this IQueryable<T> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<IReadOnlyList<TResult>> AsGraphQl<T, TResult>(this IQueryable<IReadOnlyList<T>> original) where TResult : IGraphQlAccepts<T>
        {
            throw new NotImplementedException();
        }

        public static IQueryable<TResult> Via<T, TResult, TKey>(this IQueryable<TKey> keyStrategy) where T : IResolutionFactory<TResult, TKey>
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IGraphQlResolver<TResult> Via<T, TResult, TKey, TModel>(this IQueryable<TKey> keyStrategy)
            where T : IResolutionFactory<TModel, TKey>
            where TResult : IGraphQlAccepts<TModel>, IGraphQlResolvable
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IGraphQlResolver<TResult> Via<T, TResult, TModel>(this IQueryable<GraphQlRoot> root)
            where T : IResolutionFactory<TModel>
            where TResult : IGraphQlAccepts<TModel>, IGraphQlResolvable
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }


        public static IQueryable<TResult> ViaMany<T, TResult, TKey>(IQueryable<TKey> keyStrategy) where T : IResolutionFactory<IReadOnlyList<TResult>, TKey>
        {
            throw new NotImplementedException();
            //return new MultiResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IGraphQlListResolver<TResult> ViaMany<T, TResult, TModel>(this IQueryable<GraphQlRoot> root)
            where T : IResolutionFactory<IReadOnlyList<TModel>>
            where TResult : IGraphQlAccepts<TModel>, IGraphQlResolvable
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IGraphQlListResolver<TResult> ViaMany<T, TResult, TKey, TModel>(this IQueryable<TKey> keyStrategy)
            where T : IResolutionFactory<IReadOnlyList<TModel>, TKey>
            where TResult : IGraphQlAccepts<TModel>, IGraphQlResolvable
        {
            throw new NotImplementedException();
            //return new SingleResolutionStrategy<T, TResult, TKey>(keyStrategy);
        }

        public static IQueryable<IReadOnlyDictionary<string, object>> Combine<T>(this IQueryable<T> original, CombineOptions<T> combineOptions)
        {
            throw new NotImplementedException();
            //return new CombinationStrategy(combineOptions);
        }

        public static ComplexResolverBuilder<T, IDictionary<string, Expression>> ResolveComplex<T>(this IGraphQlResolver<T> original)
            where T : IGraphQlResolvable
        {
            return new ComplexResolverBuilder<T, IDictionary<string, Expression>>(original.Query, original);
        }

        public static ComplexResolverBuilder<T, IReadOnlyList<IDictionary<string, Expression>>> ResolveComplex<T>(this IGraphQlListResolver<T> original)
            where T : IGraphQlResolvable
        {
            return new ComplexResolverBuilder<T, IReadOnlyList<IDictionary<string, Expression>>>(original.Query, original);
        }

        public class ComplexResolverBuilder<T, TFinal>
            where T : IGraphQlResolvable
        {
            private readonly IQueryable original;
            private readonly IGraphQlResolvableProducer<T> producer;
            private readonly ImmutableDictionary<string, Expression> expressions;

            public ComplexResolverBuilder(IQueryable original, IGraphQlResolvableProducer<T> producer)
            {
                this.original = original;
                this.producer = producer;
                this.expressions = ImmutableDictionary<string, Expression>.Empty;
            }

            protected ComplexResolverBuilder(IQueryable original, IGraphQlResolvableProducer<T> producer, ImmutableDictionary<string, Expression> expressions)
            {
                this.original = original;
                this.producer = producer;
                this.expressions = expressions;
            }

            public ComplexResolverBuilder<T, TFinal> Add(string property, params object[] parameters) => Add(property, property, parameters);

            public ComplexResolverBuilder<T, TFinal> Add(string displayName, string property, params object[] parameters)
            {
                var temp = producer.ProduceResolver();
                if (temp is IGraphQlAccepts accepts)
                {
                    accepts.Original = original;
                }
                var queryable = (IQueryable)temp.ResolveQuery(property, parameters: parameters);
                var expression = queryable.Expression;

                // TODO
                return new ComplexResolverBuilder<T, TFinal>(original, producer, expressions
                    .Add(displayName, expression));
            }


            public ComplexResolverBuilder<T, TFinal> Add(string displayName, Expression<Func<T, object>> resolve)
            {
                //resolve.Body
                return new ComplexResolverBuilder<T, TFinal>(original, producer, expressions
                    .Add(displayName, resolve));
            }

            public IQueryable<TFinal> Build()
            {
                return new Query<TFinal>(original.Provider);
            }
        }
    }
}
