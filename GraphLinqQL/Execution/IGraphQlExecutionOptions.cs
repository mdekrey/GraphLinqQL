using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutionOptions
    {
        Type? Query { get; }
        Type? Mutation { get; }
        Type? Subscription { get; }
        IReadOnlyList<IGraphQlDirective> Directives { get; }
        IGraphQlTypeResolver TypeResolver { get; }
    }
}
