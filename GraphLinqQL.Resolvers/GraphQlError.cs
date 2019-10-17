using System.Collections.Generic;
using System.Linq;

namespace GraphLinqQL
{
    public class GraphQlError
    {
        public GraphQlError(string errorCode, Dictionary<string, object>? arguments = null, IReadOnlyList<QueryLocation>? locations = null)
        {
            ErrorCode = errorCode;
            Arguments = arguments ?? new Dictionary<string, object>();
            Locations = locations?.ToList() ?? new List<QueryLocation>();
        }

        public string ErrorCode { get; }
        public Dictionary<string, object> Arguments { get; }
        public List<QueryLocation> Locations { get; }
    }
}