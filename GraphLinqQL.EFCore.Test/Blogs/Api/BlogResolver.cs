using System.Collections.Generic;
using GraphLinqQL;

namespace GraphLinqQL.Blogs.Api
{
    internal class BlogResolver : Blog.GraphQlContract<Data.Blog>
    {
        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(blog => blog.BlogId.ToString());

        public override IGraphQlObjectResult<IEnumerable<Post>?> Posts() =>
            this.Resolve(blog => blog.Posts).Nullable(_ => _.List(post => post.AsContract<PostResolver>()));

        public override IGraphQlScalarResult<string> Url() =>
            this.Resolve(blog => blog.Url);
    }
}