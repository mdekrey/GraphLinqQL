using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlAccepts
    {
        IQueryable Original { set; }
    }

    public interface IGraphQlAccepts<T> : IGraphQlAccepts
    {
        new IQueryable<T> Original { get; set; }
    }
}