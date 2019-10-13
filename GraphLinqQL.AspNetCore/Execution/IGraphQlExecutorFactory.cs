namespace GraphLinqQL.Execution
{
    public interface IGraphQlExecutorFactory
    {
        IGraphQlExecutor Create();
        IGraphQlExecutor Create(string name);
    }
}