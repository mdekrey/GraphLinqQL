using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public class FieldContext
    {
        public FieldContext(IReadOnlyList<QueryLocation> locations)
        {
            Locations = locations;
        }

        public IReadOnlyList<QueryLocation> Locations { get; }
    }
}
