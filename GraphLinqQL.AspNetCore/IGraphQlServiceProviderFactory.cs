namespace GraphLinqQL
{
    public interface IGraphQlServiceProviderFactory
    {
        IGraphQlExecutionServiceProvider GetServiceProvider(GraphQlOptions options);
    }
}