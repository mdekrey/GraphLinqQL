using System;
using System.Linq;

namespace GraphQlResolver
{
    public static class ResolutionStrategyExtensions
    {

        public static IQueryable<TNext> Then<TResult, TNext>(this IQueryable<TResult> original, Func<IQueryable<TResult>, IQueryable<TNext>> p) => p(original);

    }
}