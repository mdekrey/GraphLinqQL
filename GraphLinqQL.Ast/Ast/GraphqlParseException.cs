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

#if !NETSTANDARD1_3

        protected GraphqlParseException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            
        }
#endif
    }
}