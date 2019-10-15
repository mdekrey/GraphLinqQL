using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.StarWars.Implementations
{
    class Conversions
    {
        private const double metersToFeetFactor = 3.28084;

        public static double MetersToFeet(double meters) =>
            meters * metersToFeetFactor;
    }
}
