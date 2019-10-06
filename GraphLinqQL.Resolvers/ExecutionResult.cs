using System.Collections.Generic;

namespace GraphLinqQL
{
    public class ExecutionResult
    {
        public ExecutionResult(bool errorDuringParse, object data, IReadOnlyList<GraphQlError> errors)
        {
            ErrorDuringParse = errorDuringParse;
            Data = data;
            Errors = errors;
        }

        public bool ErrorDuringParse { get; }
        public object Data { get; }
        public IReadOnlyList<GraphQlError> Errors { get; }
    }
}