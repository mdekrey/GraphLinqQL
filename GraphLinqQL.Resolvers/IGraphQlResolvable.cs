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
        T GetParameter<T>(string parameter);
    }

    public class BasicParameterResolver : IGraphQlParameterResolver
    {
        public static readonly IGraphQlParameterResolver Empty = new BasicParameterResolver(ImmutableDictionary<string, IGraphQlParameterInfo>.Empty);

        private readonly IDictionary<string, IGraphQlParameterInfo> parameters;

        public BasicParameterResolver(IDictionary<string, IGraphQlParameterInfo> parameters)
        {
            this.parameters = parameters.ToImmutableDictionary();
        }

        public T GetParameter<T>(string parameter) => (T)parameters[parameter].BindTo(typeof(T))!;

        public bool HasParameter(string parameter) => parameters.ContainsKey(parameter);
    }
}