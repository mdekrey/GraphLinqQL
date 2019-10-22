using System;

namespace GraphLinqQL.Resolution
{
    public interface IGraphQlParameterInfo
    {
        object? BindTo(Type type);
    }
}