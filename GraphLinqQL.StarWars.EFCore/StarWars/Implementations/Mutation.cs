using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWars.Domain;

namespace GraphLinqQL.StarWars.Implementations
{
    public class Mutation : Interfaces.Mutation.GraphQlContract<GraphQlRoot>
    {
        private readonly StarWarsContext dbContext;

        public Mutation(StarWarsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override IGraphQlObjectResult<Interfaces.Review?> createReview(FieldContext fieldContext, Interfaces.Episode? episode, Interfaces.ReviewInput review)
        {
            return Original.ResolveTask(async _ =>
            {
                var newReview = new Domain.Review { Stars = review.stars, Episode = InterfaceToDomain.ConvertEpisode(episode.Value), Commentary = review.commentary };
                dbContext.Add(newReview);
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
                return newReview;
            }).AsContract<Review>();
        }
    }
}
