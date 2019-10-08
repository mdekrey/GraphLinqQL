using GraphLinqQL.Ast.Nodes;
using System;

namespace GraphLinqQL.Ast
{
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public class GraphqlParseException : Exception
    {
        public GraphqlParseException()
        {
        }

        public GraphqlParseException(string message) : base(message)
        {
        }

        public GraphqlParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GraphqlParseException(string message, LocationRange locationRange) : base(message)
        {
            LocationRange = locationRange;
        }

        public GraphqlParseException(string message, LocationRange locationRange, Exception innerException) : base(message, innerException)
        {
            LocationRange = locationRange;
        }

        public LocationRange LocationRange { get; internal set; }

#if !NETSTANDARD1_3

        protected GraphqlParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(info, streamingContext)
        {
            LocationRange = new LocationRange(
                start: new Location(info.GetInt32("StartLine"), info.GetInt32("StartColumn")),
                stop: new Location(info.GetInt32("StopLine"), info.GetInt32("StopColumn"))
            );
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("StartLine", LocationRange.Start.Line);
            info.AddValue("StartColumn", LocationRange.Start.Column);
            info.AddValue("StopLine", LocationRange.Stop.Line);
            info.AddValue("StopColumn", LocationRange.Stop.Column);
            base.GetObjectData(info, context);
        }
#endif
    }
}