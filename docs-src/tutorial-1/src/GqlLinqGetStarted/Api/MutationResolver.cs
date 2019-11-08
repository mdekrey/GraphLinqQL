using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL;

namespace GqlLinqGetStarted.Api
{
    // ###Declaration
    public class MutationResolver : Mutation.GraphQlContract<GraphLinqQL.GraphQlRoot>
    // Declaration###
    {
        // ###AddBlogImplementation
        public override IGraphQlObjectResult<Blog?> AddBlog(string url) =>
            this.ResolveTask(async _ =>
            {
                var blog = new Data.Blog { BlogId = Guid.NewGuid(), Url = url };
                Data.BloggingData.Blogs.Add(blog);
                await Task.Yield();
                return blog;
            }).Nullable(blog => blog.AsContract<BlogResolver>());
        // AddBlogImplementation###

        // ###AddPostImplementation
        public override IGraphQlObjectResult<Post?> AddPost(string blogId, NewPost newPost) =>
            this.ResolveTask(async _ =>
            {
                var blog = Data.BloggingData.Blogs.FirstOrDefault(blog => blog.BlogId == Guid.Parse(blogId));
                if (blog == null) {
                    return null;
                }
                var post = new Data.Post { PostId = Guid.NewGuid(), Title = newPost.Title, Content = newPost.PostContent };
                blog.Posts.Add(post);
                await Task.Yield();
                return post;
            }).Nullable(post => post.AsContract<PostResolver>());
        // AddPostImplementation###
    }
}
