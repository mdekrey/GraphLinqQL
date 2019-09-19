using GraphQlResolver;
using GraphQlResolver.Execution;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQl(this IServiceCollection services, Action<GraphQlOptions> optionFactory)
        {
            services.AddGraphQl(Microsoft.Extensions.Options.Options.DefaultName, optionFactory);
        }

        public static void AddGraphQl(this IServiceCollection services, string optionsName, Action<GraphQlOptions> optionFactory)
        {
            services.Configure(optionsName, optionFactory);
            services.TryAddTransient<IGraphQlExecutorFactory, GraphQlExecutorFactory>();
            services.TryAddTransient<IGraphQlParameterResolverFactory, BasicParameterResolverFactory>();
        }

        public static void AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(this IServiceCollection services, Action<GraphQlOptions>? optionFactory = null)
        {
            services.AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(Microsoft.Extensions.Options.Options.DefaultName, optionFactory);
        }

        public static void AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(this IServiceCollection services, string optionsName, Action<GraphQlOptions>? optionFactory = null)
        {
            services.AddGraphQl(optionsName, options =>
            {
                options.Query = typeof(TQuery);
                options.Mutation = typeof(TMutation);
                options.TypeResolver = typeof(TGraphQlTypeResolver);
                optionFactory?.Invoke(options);
            });
        }
    }
}
