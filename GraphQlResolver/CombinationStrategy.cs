namespace GraphQlResolver
{
    internal class CombinationStrategy : IResolutionStrategy<object>
    {
        private CombineOptions combineOptions;

        public CombinationStrategy(CombineOptions combineOptions)
        {
            this.combineOptions = combineOptions;
        }
    }
}