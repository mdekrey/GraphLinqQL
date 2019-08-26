using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        IQueryable ResolveQuery(string name, params object[] parameters);
    }
}