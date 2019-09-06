using GraphQlResolver.Directives;
using GraphQlResolver.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQl(this IServiceCollection services, Action<GraphQlOptions> optionFactory)
        {
            services.AddGraphQl(Options.DefaultName, optionFactory);
        }

        public static void AddGraphQl(this IServiceCollection services, string optionsName, Action<GraphQlOptions> optionFactory)
        {
            services.Configure(optionsName, optionFactory);
            services.TryAddTransient<IGraphQlExecutorFactory, GraphQlExecutorFactory>();
        }

        public static void AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(this IServiceCollection services, Action<GraphQlOptions>? optionFactory = null)
        {
            services.AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(Options.DefaultName, optionFactory);
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
