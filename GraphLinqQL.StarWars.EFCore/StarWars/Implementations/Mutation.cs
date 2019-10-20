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

        public override IGraphQlObjectResult<Interfaces.Review?> CreateReview(Interfaces.Episode? episode, Interfaces.ReviewInput review)
        {
            return Original.ResolveTask(async _ =>
            {
                var newReview = new Domain.Review { Stars = review.Stars, Episode = InterfaceToDomain.ConvertEpisode(episode!.Value), Commentary = review.Commentary };
                dbContext.Add(newReview);
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
                return newReview;
            }).AsContract<Review>();
        }
    }
}
