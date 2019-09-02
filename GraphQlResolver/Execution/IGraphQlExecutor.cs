using System.Collections.Generic;

namespace GraphQlResolver.Execution
{
    public interface IGraphQlExecutor
    {
        object Execute(string query, IDictionary<string, object> arguments);
    }
}