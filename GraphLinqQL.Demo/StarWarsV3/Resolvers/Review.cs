using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlScalarResult<string?> commentary(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<Episode?> episode(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }

        public override IGraphQlScalarResult<int> stars(FieldContext fieldContext)
        {
            throw new System.NotImplementedException();
        }
    }

}
