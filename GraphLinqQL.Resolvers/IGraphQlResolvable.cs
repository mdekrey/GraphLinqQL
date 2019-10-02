using System;
using System.Collections.Generic;

namespace GraphLinqQL
{
    public interface IGraphQlResolvable
    {
        IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters);
        bool IsType(string value);
    }

    public interface IGraphQlParameterInfo
    {
        object? BindTo(Type type);
    }

    public interface IGraphQlParameterResolverFactory
    {
        IGraphQlParameterResolver FromParameterData(IDictionary<string, IGraphQlParameterInfo> rawData);
    }

    public interface IGraphQlParameterResolver
    {
        bool HasParameter(string parameter);
        T GetParameter<T>(string parameter);
    }
}