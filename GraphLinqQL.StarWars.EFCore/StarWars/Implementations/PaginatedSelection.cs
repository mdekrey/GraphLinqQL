using GraphLinqQL.Resolution;
using GraphLinqQL.StarWars.Domain;
using System;
using System.Linq;

namespace GraphLinqQL.StarWars.Implementations
{
    [InlinableClass]
    internal class PaginatedSelection<T>
    {
#nullable disable warnings
        public IQueryable<T> AllData { get; set; }
        public IQueryable<T> SkippedData { get; set; }
        public int Take { get; set; }
    }
}