namespace GraphLinqQL.Resolution
{
    public interface IGraphQlParameterResolver
    {
        bool HasParameter(string parameter);
        T GetParameter<T>(string parameter);
    }
}