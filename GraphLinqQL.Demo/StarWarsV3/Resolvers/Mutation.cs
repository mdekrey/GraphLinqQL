using System;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    public class Mutation : Interfaces.Mutation.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<Interfaces.Review?> createReview(FieldContext fieldContext, Episode? episode, ReviewInput review)
        {
            if (episode == null)
            {
                throw new ArgumentNullException(nameof(episode));
            }
            return Original.ResolveTask(async _ =>
            {
                await Task.Yield();
                var ep = InterfaceToDomain.ConvertEpisode(episode.Value);
                var newReview = new Domain.Review { Episode = ep, Commentary = review.commentary, Stars = review.stars };
                Domain.Data.reviews[ep].Add(newReview);
                return newReview;
            }).AsContract<Review>();
        }
    }
}
