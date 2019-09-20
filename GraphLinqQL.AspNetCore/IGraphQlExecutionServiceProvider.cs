using System;

namespace GraphLinqQL
{
    public interface IGraphQlExecutionServiceProvider : IGraphQlServiceProvider
    {
        IGraphQlDirective GetDirective(Type directive);
    }
}