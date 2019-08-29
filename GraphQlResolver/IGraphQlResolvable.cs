using System.Linq;
using GraphQlSchema;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        IGraphQlResult ResolveQuery(string name, params object[] parameters);
    }
}