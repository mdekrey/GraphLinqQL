using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GqlLinqGetStarted
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ###EF Core Services
            services.AddDbContext<Data.BloggingContext>();
            services.AddHostedService<Data.DbContextInitializingHostedService>();
            // EF Core Services###

            // ###GraphLinqQL Services
            services.AddGraphQl<Api.TypeResolver>(typeof(Api.QueryResolver), options =>
            {
                options.Mutation = typeof(Api.MutationResolver);
                options.AddIntrospection();
            });
            // GraphLinqQL Services###

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                /// ###GraphLinqQL Endpoint
                endpoints.UseGraphQl("/graphql");
                /// GraphLinqQL Endpoint###
            });
        }
    }
}
