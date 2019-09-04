using System.Collections.Generic;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        IGraphQlResult ResolveQuery(string name, IDictionary<string, object?> parameters);
        bool IsType(string value);
    }
}