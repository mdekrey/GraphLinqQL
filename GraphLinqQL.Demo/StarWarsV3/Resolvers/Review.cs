using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlScalarResult<string?> Commentary() =>
            this.Original().Resolve(review => review.Commentary);

        public override IGraphQlScalarResult<Episode?> Episode() =>
            this.Original().Resolve(review => (Episode?)DomainToInterface.ConvertEpisode(review.Episode));

        public override IGraphQlScalarResult<int> Stars() =>
            this.Original().Resolve(review => review.Stars);
    }

}
