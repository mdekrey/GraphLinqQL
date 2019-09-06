namespace GraphQlResolver.Execution
{
    internal interface IGraphQlExecutorFactory
    {
        IGraphQlExecutor Create();
        IGraphQlExecutor Create(string name);
    }
}