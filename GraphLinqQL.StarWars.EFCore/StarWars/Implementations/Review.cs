using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWars.Interfaces;

namespace GraphLinqQL.StarWars.Implementations
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlScalarResult<string?> commentary() =>
            Original.Resolve(_ => _.Commentary);

        public override IGraphQlScalarResult<Episode?> episode() =>
            Original.Resolve(_ => (Episode?)DomainToInterface.ConvertEpisode(_.Episode));

        public override IGraphQlScalarResult<int> stars() =>
            Original.Resolve(_ => _.Stars);
    }
}
