using GraphLinqQL.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Execution
{
    public static class ExceptionExtensions
    {
        public static void ConvertAstExceptions(this Exception ex)
        {
            if (ex is GraphqlParseException parseException)
            {
                ex.AddGraphQlError(WellKnownErrorCodes.ParseError, parseException.LocationRange.ToQueryLocations(), new { innerMessage = ex.Message });
            }
            else if (ex is AggregateException aggregateException)
            {
                foreach (var inner in aggregateException.InnerExceptions)
                {
                    inner.ConvertAstExceptions();
                }
            }
        }
    }
}
