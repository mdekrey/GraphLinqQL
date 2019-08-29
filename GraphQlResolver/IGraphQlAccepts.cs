using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlAccepts<T>
    {
        IGraphQlResultFactory<T> Original { get; set; }
    }
}