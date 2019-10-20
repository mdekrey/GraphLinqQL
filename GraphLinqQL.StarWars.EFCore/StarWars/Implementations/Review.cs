using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWars.Interfaces;

namespace GraphLinqQL.StarWars.Implementations
{
    class Review : Interfaces.Review.GraphQlContract<Domain.Review>
    {
        public override IGraphQlScalarResult<string?> Commentary() =>
            Original.Resolve(_ => _.Commentary);

        public override IGraphQlScalarResult<Episode?> Episode() =>
            Original.Resolve(_ => (Episode?)DomainToInterface.ConvertEpisode(_.Episode));

        public override IGraphQlScalarResult<int> Stars() =>
            Original.Resolve(_ => _.Stars);
    }
}
