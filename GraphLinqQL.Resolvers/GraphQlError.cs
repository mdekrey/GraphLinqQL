using System.Collections.Generic;

namespace GraphLinqQL
{
    public class GraphQlError
    {
        public GraphQlError(string errorCode, string message, IReadOnlyList<QueryLocation> locations)
        {
            ErrorCode = errorCode;
            Message = message;
            Locations = locations;
        }

        public string ErrorCode { get; }
        public string Message { get; }
        public IReadOnlyList<QueryLocation> Locations { get; }
    }
}