using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL;

namespace GqlLinqGetStarted.Api
{
    // ###Declaration
    public class MutationResolver : Mutation.GraphQlContract<GraphLinqQL.GraphQlRoot>
    {
        private readonly Data.BloggingContext context;

        public MutationResolver(Data.BloggingContext context)
        {
            this.context = context;
        }
    // Declaration###

        // ###AddBlogImplementation
        public override IGraphQlObjectResult<Blog?> AddBlog(string url) =>
            this.ResolveTask(async _ =>
            {
                var blog = new Data.Blog { Url = url };
                context.Add(blog);
                await context.SaveChangesAsync();
                return blog;
            }).Nullable(blog => blog.AsContract<BlogResolver>());
        // AddBlogImplementation###

        // ###AddPostImplementation
        public override IGraphQlObjectResult<Post?> AddPost(string blogId, NewPost newPost) =>
            this.ResolveTask(async _ =>
            {
                var post = new Data.Post { BlogId = int.Parse(blogId), Title = newPost.Title, Content = newPost.PostContent };
                context.Add(post);
                await context.SaveChangesAsync();
                return post;
            }).Nullable(post => post.AsContract<PostResolver>());
        // AddPostImplementation###
    }
}
