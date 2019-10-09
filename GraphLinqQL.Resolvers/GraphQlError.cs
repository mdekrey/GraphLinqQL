using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL
{
    public class GraphQlError
    {
        public GraphQlError(string errorCode, IDictionary<string, object> arguments, IReadOnlyList<QueryLocation> locations)
        {
            ErrorCode = errorCode;
            Arguments = arguments.ToImmutableDictionary();
            Locations = locations;
        }

        public string ErrorCode { get; }
        public IDictionary<string, object> Arguments { get; }
        public IReadOnlyList<QueryLocation> Locations { get; }
    }
}