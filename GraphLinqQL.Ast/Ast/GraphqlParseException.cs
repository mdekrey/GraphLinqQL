using System;

namespace GraphLinqQL.Ast
{
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
    }
}