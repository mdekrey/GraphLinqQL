using System;

namespace GraphLinqQL
{
    public interface IGraphQlExecutionServiceProvider : IGraphQlServiceProvider
    {
        IServiceProvider ExecutionServices { get; }
        IGraphQlDirective GetDirective(Type directive);
    }
}