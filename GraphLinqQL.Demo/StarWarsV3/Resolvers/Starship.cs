using System.Collections.Generic;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Starship : Interfaces.Starship.GraphQlContract<Domain.Starship>
    {
        public override IGraphQlScalarResult<IEnumerable<IEnumerable<double>>?> Coordinates() =>
            this.Original().Resolve<IEnumerable<IEnumerable<double>> ? >(_ => new double[][] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });

        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(starship => starship.Id);

        public override IGraphQlScalarResult<double?> Length(LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.Meter) == LengthUnit.Meter ? 1 : 3.28084;
            return this.Original().Resolve<double?>(starship => starship.Length * unitFactor);
        }

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(starship => starship.Name);
    }

}
