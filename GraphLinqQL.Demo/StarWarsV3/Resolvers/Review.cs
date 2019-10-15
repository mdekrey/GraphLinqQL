using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlScalarResult<string?> commentary(FieldContext fieldContext) =>
            Original.Resolve(review => review.Commentary);

        public override IGraphQlScalarResult<Episode?> episode(FieldContext fieldContext) =>
            Original.Resolve(review => (Episode?)DomainToInterface.ConvertEpisode(review.Episode));

        public override IGraphQlScalarResult<int> stars(FieldContext fieldContext) =>
            Original.Resolve(review => review.Stars);
    }

}
