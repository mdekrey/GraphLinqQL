using System;
using System.Collections.Generic;
#nullable disable warnings

// ###DataNamespace
namespace GqlLinqGetStarted.Data
{
    public static class BloggingData
    {
        public static List<Blog> Blogs { get; } = new List<Blog>();
    }

    public class Blog
    {
        public Guid BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }
    }
}
// DataNamespace###