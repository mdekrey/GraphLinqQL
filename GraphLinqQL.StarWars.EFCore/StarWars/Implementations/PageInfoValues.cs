using System;
using System.Threading.Tasks;

namespace GraphLinqQL.StarWars.Implementations
{
    internal class PageInfoValues
    {
#nullable disable warnings
        public Func<Task<string>> EndCursor { get; set; }
        public Func<Task<string>> StartCursor { get; set; }
        public Func<Task<bool>> HasNextPage { get; set; }
    }
}