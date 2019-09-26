using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLinqQL.StarWarsV4.Resolvers
{
    public class Pagination<T>
    {
        public IQueryable<T> Unpaginated { get; }
        public IQueryable<T> Paginated { get; }

        public Pagination(IQueryable<T> unpaginated, IQueryable<T> paginated)
        {
            this.Unpaginated = unpaginated;
            this.Paginated = paginated;
        }

    }

    public static class PaginationExtensions
    {
        public static Pagination<T> Paginate<T>(this IQueryable<T> unpaginated, Func<IQueryable<T>, IQueryable<T>> paginate)
        {
            return new Pagination<T>(unpaginated, paginate(unpaginated));
        }
    }
}
