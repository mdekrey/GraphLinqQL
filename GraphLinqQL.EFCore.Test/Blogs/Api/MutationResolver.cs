﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphLinqQL;

namespace GraphLinqQL.Blogs.Api
{
    public class MutationResolver : Mutation.GraphQlContract<GraphLinqQL.GraphQlRoot>
    {
        private readonly Data.BloggingContext context;

        public MutationResolver(Data.BloggingContext context)
        {
            this.context = context;
        }

        public override IGraphQlObjectResult<Blog?> AddBlog(string url) =>
            this.ResolveTask(async _ =>
            {
                var blog = new Data.Blog { Url = url };
                context.Add(blog);
                await context.SaveChangesAsync().ConfigureAwait(false);
                return blog;
            }).Nullable(blog => blog.AsContract<BlogResolver>());

        public override IGraphQlObjectResult<Post?> AddPost(string blogId, NewPost newPost) =>
            this.ResolveTask(async _ =>
            {
                var post = new Data.Post { BlogId = int.Parse(blogId), Title = newPost.Title, Content = newPost.PostContent };
                context.Add(post);
                await context.SaveChangesAsync().ConfigureAwait(false);
                return post;
            }).Nullable(post => post.AsContract<PostResolver>());
    }
}
