using GraphLinqQL;

namespace GqlLinqGetStarted.Api
{
    internal class PostResolver : Post.GraphQlContract<Data.Post>
    {
        public override IGraphQlScalarResult<string> Content() =>
            this.Resolve(post => post.Content);

        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(post => post.PostId.ToString());

        public override IGraphQlScalarResult<string> Title() =>
            this.Resolve(post => post.Title);
    }
}