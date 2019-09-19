using System;
using System.Collections.Generic;

namespace GraphQlResolver
{
    public interface IGraphQlResolvable
    {
        IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters);
        bool IsType(string value);
    }

    public interface IGraphQlParameterResolverFactory
    {
        IGraphQlParameterResolver FromParameterData(IDictionary<string, string> rawData);
    }

    public interface IGraphQlParameterResolver
    {
        bool HasParameter(string parameter);
        string GetRawParameter(string parameter);
        T GetParameter<T>(string parameter);
    }
}