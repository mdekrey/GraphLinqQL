using System.Collections.Generic;
using GraphLinqQL;

namespace GqlLinqGetStarted.Api
{
    // ###BlogResolver
    internal class BlogResolver : Blog.GraphQlContract<Data.Blog>
    {
        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(blog => blog.BlogId.ToString());

        public override IGraphQlScalarResult<string> Url() =>
            this.Resolve(blog => blog.Url);

        public override IGraphQlObjectResult<IEnumerable<Post>?> Posts() =>
            this.Resolve(blog => blog.Posts).Nullable(_ => _.List(post => post.AsContract<PostResolver>()));
    }
    // BlogResolver###
}