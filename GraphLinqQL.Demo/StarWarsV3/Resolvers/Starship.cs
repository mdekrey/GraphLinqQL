using System.Collections.Generic;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Starship : Interfaces.Starship.GraphQlContract<Domain.Starship>
    {
        public override IGraphQlScalarResult<IEnumerable<IEnumerable<double>>?> coordinates(FieldContext fieldContext) =>
            Original.Resolve<IEnumerable<IEnumerable<double>> ? >(_ => new double[][] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });

        public override IGraphQlScalarResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(starship => starship.Id);

        public override IGraphQlScalarResult<double?> length(FieldContext fieldContext, LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.METER) == LengthUnit.METER ? 1 : 3.28084;
            return Original.Resolve<double?>(starship => starship.Length * unitFactor);
        }

        public override IGraphQlScalarResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(starship => starship.Name);
    }

}
