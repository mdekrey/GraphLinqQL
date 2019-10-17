﻿using System;
using System.Collections.Generic;
using System.Text;
using GraphLinqQL.StarWars.Interfaces;

namespace GraphLinqQL.StarWars.Implementations
{
    class Starship : Interfaces.Starship.GraphQlContract<Domain.Starship>
    {
        public override IGraphQlScalarResult<IEnumerable<IEnumerable<double>>?> coordinates()
        {
            return Original.Resolve(new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } });
        }

        public override IGraphQlScalarResult<string> id() =>
            Original.Resolve(starship => starship.Id.ToString());

        public override IGraphQlScalarResult<double?> length(LengthUnit? unit)
        {
            if (unit == LengthUnit.FOOT)
            {
                return Original.Resolve(starship => (double?)Conversions.MetersToFeet(starship.Length));
            }
            else
            {
                return Original.Resolve(starship => (double?)starship.Length);
            }
        }

        public override IGraphQlScalarResult<string> name() =>
            Original.Resolve(starship => starship.Name);
    }
}
