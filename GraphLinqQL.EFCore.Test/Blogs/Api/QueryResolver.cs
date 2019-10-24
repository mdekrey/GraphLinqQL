using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL;

namespace GraphLinqQL.Blogs.Api
{
    public class QueryResolver : Query.GraphQlContract<GraphQlRoot>
    {
        private readonly Data.BloggingContext context;

        public QueryResolver(Data.BloggingContext context)
        {
            this.context = context;
        }

        public override IGraphQlObjectResult<IEnumerable<Blog>?> Blogs() =>
            this.Resolve(_ => context.Blogs).Nullable(_ => _.List(blog => blog.AsContract<BlogResolver>()));
    }
}
