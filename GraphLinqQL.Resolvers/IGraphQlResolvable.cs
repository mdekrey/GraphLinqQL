using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL
{
    public interface IGraphQlResolvable
    {
        string GraphQlTypeName { get; }
        IGraphQlResult ResolveQuery(string name, FieldContext fieldContext, IGraphQlParameterResolver parameters);
        bool IsType(string value);
    }

    public interface IGraphQlParameterInfo
    {
        object? BindTo(Type type);
    }

    public interface IGraphQlParameterResolver
    {
        bool HasParameter(string parameter);
        T GetParameter<T>(string parameter, FieldContext fieldContext);
    }
}