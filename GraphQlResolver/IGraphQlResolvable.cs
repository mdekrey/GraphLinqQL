using System.Collections.Generic;
using System.Linq;
using GraphQlSchema;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        IGraphQlResult ResolveQuery(string name, IDictionary<string, object> parameters);
    }
}