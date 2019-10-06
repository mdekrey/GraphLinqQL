using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Execution
{
    public static class AstExtensions
    {

        public static System.Collections.Generic.IReadOnlyList<QueryLocation> ToQueryLocations(this Ast.Nodes.LocationRange location)
        {
            return location.Start.Line == 0 && location.Start.Column == 0
                ? EmptyArrayHelper.Empty<QueryLocation>()
                : new[] { new QueryLocation(location.Start.Line, location.Start.Column) };
        }
    }
}
