using System;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    public class Mutation : Interfaces.Mutation.GraphQlContract<GraphQlRoot>
    {
        public override IGraphQlResult<Interfaces.Review?> createReview(FieldContext fieldContext, Episode? episode, ReviewInput review)
        {
            throw new NotImplementedException();
        }
    }
}
