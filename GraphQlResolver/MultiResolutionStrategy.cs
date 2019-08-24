using System.Collections.Generic;

namespace GraphQlResolver
{
    internal class MultiResolutionStrategy<T, TResult> : IResolutionStrategy<TResult> where T : IResolutionFactory<IList<TResult>>
    {
    }
    internal class MultiResolutionStrategy<T, TResult, TKey> : IResolutionStrategy<TResult> where T : IResolutionFactory<IList<TResult>, TKey>
    {
        private IResolutionStrategy<TKey> keyStrategy;

        public MultiResolutionStrategy(IResolutionStrategy<TKey> keyStrategy)
        {
            this.keyStrategy = keyStrategy;
        }
    }
}