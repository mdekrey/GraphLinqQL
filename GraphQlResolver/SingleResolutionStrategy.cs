namespace GraphQlResolver
{
    internal class SingleResolutionStrategy<T, TResult> : IResolutionStrategy<TResult> where T : IResolutionFactory<TResult>
    {
    }

    internal class SingleResolutionStrategy<T, TResult, TKey> : IResolutionStrategy<TResult> where T : IResolutionFactory<TResult, TKey>
    {
        private IResolutionStrategy<TKey> keyStrategy;

        public SingleResolutionStrategy(IResolutionStrategy<TKey> keyStrategy)
        {
            this.keyStrategy = keyStrategy;
        }
    }
}