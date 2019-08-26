using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlAccepts<T>
    {
        IQueryable<T> Original { get; set; }
    }
}