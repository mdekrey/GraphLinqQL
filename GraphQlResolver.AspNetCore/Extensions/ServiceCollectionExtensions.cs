using GraphQlResolver.Directives;
using GraphQlResolver.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQl<TQuery, TMutation, TGraphQlTypeResolver>(this IServiceCollection services)
            where TQuery : class, IGraphQlResolvable
            where TMutation : class, IGraphQlResolvable
            where TGraphQlTypeResolver : class, IGraphQlTypeResolver
        {
            services.AddTransient<GraphQlExecutor<TQuery, TMutation, TGraphQlTypeResolver>>();
            services.AddTransient<TQuery>();
            services.AddTransient<TMutation>();
            services.AddTransient<TGraphQlTypeResolver>();
            // TODO - make this function return a builder to make it easy to add directives
            services.AddTransient<IGraphQlDirective, SkipDirective>();
            services.AddTransient<IGraphQlDirective, IncludeDirective>();
        }

    }
}
