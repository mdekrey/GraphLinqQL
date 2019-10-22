using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GraphLinqQL.Resolution
{
    public interface IGraphQlResolvable
    {
        FieldContext FieldContext { get; set; }
        string GraphQlTypeName { get; }
        IGraphQlResult ResolveQuery(string name, IGraphQlParameterResolver parameters);
        bool IsType(string value);
    }
}