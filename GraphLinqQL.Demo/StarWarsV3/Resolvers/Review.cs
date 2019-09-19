using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlResult<string?> commentary()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Episode?> episode()
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<int> stars()
        {
            throw new System.NotImplementedException();
        }
    }

}
