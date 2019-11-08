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
        // ###BlogsImplementation
        public override IGraphQlObjectResult<IEnumerable<Blog>?> Blogs() =>
            this.Resolve(_ => Data.BloggingData.Blogs)
                .Nullable(_ => _.List(blog => blog.AsContract<BlogResolver>()));
        // BlogsImplementation###
    }
}
