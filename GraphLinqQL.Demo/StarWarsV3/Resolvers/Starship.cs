using System.Collections.Generic;
using GraphLinqQL.StarWarsV3.Interfaces;

namespace GraphLinqQL.StarWarsV3.Resolvers
{
    class Starship : Interfaces.Starship.GraphQlContract<Domain.Starship>
    {
        public override IGraphQlResult<IEnumerable<IEnumerable<double>>?> coordinates(FieldContext fieldContext) =>
            Original.Resolve< IEnumerable<IEnumerable<double>> ? >(_ => new double[][] { new[] { 1.0, 2 }, new[] { 3.0, 4 } });

        public override IGraphQlResult<string> id(FieldContext fieldContext) =>
            Original.Resolve(starship => starship.Id);

        public override IGraphQlResult<double?> length(FieldContext fieldContext, LengthUnit? unit)
        {
            var unitFactor = (unit ?? LengthUnit.METER) == LengthUnit.METER ? 1 : 3.28084;
            return Original.Resolve<double?>(starship => starship.Length * unitFactor);
        }

        public override IGraphQlResult<string> name(FieldContext fieldContext) =>
            Original.Resolve(starship => starship.Name);
    }

}
