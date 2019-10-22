using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWars.Interfaces;

namespace GraphLinqQL.StarWars.Implementations
{
    class Starship : Interfaces.Starship.GraphQlContract<Domain.Starship>
    {
        public override IGraphQlScalarResult<IEnumerable<IEnumerable<double>>?> Coordinates()
        {
            return this.Resolve(new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } });
        }

        public override IGraphQlScalarResult<string> Id() =>
            this.Resolve(starship => starship.Id.ToString());

        public override IGraphQlScalarResult<double?> Length(LengthUnit? unit)
        {
            if (unit == LengthUnit.Foot)
            {
                return this.Resolve(starship => (double?)Conversions.MetersToFeet(starship.Length));
            }
            else
            {
                return this.Resolve(starship => (double?)starship.Length);
            }
        }

        public override IGraphQlScalarResult<string> Name() =>
            this.Resolve(starship => starship.Name);
    }
}
