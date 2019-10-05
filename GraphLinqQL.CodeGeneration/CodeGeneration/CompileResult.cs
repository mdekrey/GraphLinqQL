using System.Collections.Generic;

namespace GraphLinqQL.CodeGeneration
{
    public readonly struct CompileResult
    {
        public CompileResult(IEnumerable<CompilerError> errors, string code)
        {
            Errors = errors;
            Code = code;
        }

        public IEnumerable<CompilerError> Errors { get; }
        public string Code { get; }
    }
}