using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQlResolver
{
    public interface IResolutionFactory<TResult>
    {
        Task<TResult> Resolve();
    }
    public interface IResolutionFactory<TResult, TKey>
    {
        Task<TResult> Resolve(TKey input);
        Task<Func<TKey, TResult>> ResolveMany(IReadOnlyList<TKey> input);
    }
}