using GraphLinqQL;
using GraphLinqQL.Ast;
using GraphLinqQL.ErrorMessages;
using GraphLinqQL.Execution;
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
            services.TryAddSingleton<IGraphQlServiceProviderFactory, GraphQlServiceProviderFactory>();
            services.TryAddSingleton<IGraphQlExecutorFactory, GraphQlExecutorFactory>();
            services.AddScoped<GraphQlCurrentServiceProvider>();
            services.TryAddScoped<IGraphQlExecutionServiceProvider>(sp => sp.GetRequiredService<GraphQlCurrentServiceProvider>().CurrentServiceProvider!);
            services.TryAddScoped<IGraphQlServiceProvider>(sp => sp.GetRequiredService<IGraphQlExecutionServiceProvider>());
            services.TryAddSingleton<IAbstractSyntaxTreeGenerator, AbstractSyntaxTreeGenerator>();
            services.TryAddSingleton<IMessageResolver, MessageResolver>();
            services.AddSingleton<IMessageProvider, WellKnownErrorCodes>();
        }

        public static void AddGraphQl<TGraphQlTypeResolver>(this IServiceCollection services, Type queryType, Action<GraphQlOptions>? optionFactory = null)
            where TGraphQlTypeResolver : IGraphQlTypeResolver
        {
            services.AddGraphQl<TGraphQlTypeResolver>(Microsoft.Extensions.Options.Options.DefaultName, queryType, optionFactory);
        }

        public static void AddGraphQl<TGraphQlTypeResolver>(this IServiceCollection services, string optionsName, Type queryType, Action<GraphQlOptions>? optionFactory = null)
        {
            services.AddGraphQl(optionsName, options =>
            {
                options.Query = queryType;
                options.TypeResolver = typeof(TGraphQlTypeResolver);
                optionFactory?.Invoke(options);
            });
        }
    }
}
