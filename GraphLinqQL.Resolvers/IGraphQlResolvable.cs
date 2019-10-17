using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL
{
    public interface IGraphQlResolvable
    {
        FieldContext FieldContext { get; set; }
        string GraphQlTypeName { get; }
        IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters);
        bool IsType(string value);
    }

    public interface IGraphQlParameterInfo
    {
        object? BindTo(Type type);
    }

    public interface IGraphQlParameterResolver
    {
        bool HasParameter(string parameter);
        T GetParameter<T>(string parameter);
    }
}