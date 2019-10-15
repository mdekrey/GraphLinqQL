using GraphLinqQL.Sample.Domain;
using System;
using System.Linq;

namespace GraphLinqQL.Sample.Implementations
{
    internal class PaginatedSelection<T>
    {
#nullable disable warnings
        public IQueryable<T> AllData { get; set; }
        public IQueryable<T> SkippedData { get; set; }
        public int Take { get; set; }
    }
}