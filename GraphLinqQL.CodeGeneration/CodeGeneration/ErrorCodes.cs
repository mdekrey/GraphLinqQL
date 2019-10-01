using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.CodeGeneration
{
    public static class ErrorCodes
    {
        public const string GraphLinqQLPrefix = "GQL";

        public const string UnknownScalarTypeErrorCode = GraphLinqQLPrefix + "0001";
        public static string UnknownScalarTypeMessage(string scalarTypeName) =>
            $"Scalar type '{scalarTypeName}' in schema is unmapped to C#";
    }
}
