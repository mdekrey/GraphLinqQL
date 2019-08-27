using System.Linq;
using GraphQlSchema;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        object ResolveQuery(string name, params object[] parameters);
    }

    public interface IGraphQlResolver
    {
        IQueryable Query { get; }
    }

    public interface IGraphQlResolvableProducer<out T>
    {
        T ProduceResolver();
    }

    public interface IGraphQlResolver<out T> : IGraphQlResolver, IGraphQlResolvableProducer<T>
        where T : IGraphQlResolvable
    {
    }


    public interface IGraphQlListResolver<out T> : IGraphQlResolver, IGraphQlResolvableProducer<T>
        where T : IGraphQlResolvable
    {
    }

    public class GraphQlResolver<T> : IGraphQlResolver<T>
        where T : IGraphQlResolvable, new()
    {
        public GraphQlResolver(IQueryable query)
        {
            this.Query = query;
        }

        public IQueryable Query { get; }

        public T ProduceResolver()
        {
            return new T(); // TODO - don't do this
        }
    }

    public class GraphQlListResolver<T> : IGraphQlListResolver<T>
        where T : IGraphQlResolvable, new()
    {
        public GraphQlListResolver(IQueryable query)
        {
            this.Query = query;
        }

        public IQueryable Query { get; }

        public T ProduceResolver()
        {
            return new T(); // TODO - don't do this
        }
    }
}