using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlResult<string?> commentary(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<Episode?> episode(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlResult<int> stars(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }

}
