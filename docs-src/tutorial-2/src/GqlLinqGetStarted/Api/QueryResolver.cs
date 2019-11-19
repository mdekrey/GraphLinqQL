using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL;

namespace GqlLinqGetStarted.Api
{
    // ###Declaration
    public class QueryResolver : Query.GraphQlContract<GraphQlRoot>
    // Declaration###
    {
        // ###DependencyInjection
        private readonly Data.BloggingContext context;

        public QueryResolver(Data.BloggingContext context)
        {
            this.context = context;
        }
        // DependencyInjection###

        // ###BlogsImplementation
        public override IGraphQlObjectResult<IEnumerable<Blog>?> Blogs() =>
            this.Resolve(_ => context.Blogs).Nullable(_ => _.List(blog => blog.AsContract<BlogResolver>()));
        // BlogsImplementation###
    }
}
