using System;
using System.Threading.Tasks;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    public class Mutation : Interfaces.Mutation.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlObjectResult<Interfaces.Review?> CreateReview(Episode? episode, ReviewInput review)
        {
            if (episode == null)
            {
                throw new ArgumentNullException(nameof(episode));
            }
            return this.Original().ResolveTask(async _ =>
            {
                await Task.Yield();
                var ep = InterfaceToDomain.ConvertEpisode(episode.Value);
                var newReview = new Domain.Review { Episode = ep, Commentary = review.Commentary, Stars = review.Stars };
                Domain.Data.reviews[ep].Add(newReview);
                return newReview;
            }).AsContract<Review>();
        }
    }
}
