using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Human : Interfaces.Human.GraphQlContract<Domain.Human>
    {
        public override IGraphQlScalarResult<IEnumerable<Episode?>> AppearsIn() =>
            this.Original().Resolve(human => human.AppearsIn.Select(DomainToInterface.ConvertEpisode).Select(ep => (Episode?) ep));

        public override IGraphQlObjectResult<IEnumerable<Character?>?> Friends() =>
            this.Original().Resolve(human => from id in human.Friends
                                      select Domain.Data.humanLookup.ContainsKey(id)
                                        ? (object)Domain.Data.humanLookup[id]
                                        : Domain.Data.droidLookup[id]).List(_ => _.AsUnion<Character>(builder => builder.Add<Domain.Human, Human>().Add<Domain.Droid, Droid>()));

        public override IGraphQlObjectResult<Interfaces.FriendsConnection> FriendsConnection(int? first, string? after) =>
            this.Original().Resolve(human => new FriendsConnection.Data(human.Friends, first, after)).AsContract<FriendsConnection>();

        public override IGraphQlScalarResult<double?> Height(LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.Meter) == LengthUnit.Meter ? 1 : 3.28084;
            return this.Original().Resolve<double?>(human => human.Height * unitFactor);
        }

        public override IGraphQlScalarResult<string?> HomePlanet() =>
            this.Original().Resolve(human => human.HomePlanet);

        public override IGraphQlScalarResult<string> Id() =>
            this.Original().Resolve(human => human.Id);

        public override IGraphQlScalarResult<double?> Mass() =>
            this.Original().Resolve<double?>(human => human.Mass);

        public override IGraphQlScalarResult<string> Name() =>
            this.Original().Resolve(human => human.Name);

        public override IGraphQlObjectResult<IEnumerable<Interfaces.Starship?>?> Starships() =>
            this.Original().Resolve(human => human.Starships.Select(id => Domain.Data.starshipLookup[id])).List(_ => _.AsContract<Starship>());
    }

}
