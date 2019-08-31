using System;
using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlAccepts
    {
        IGraphQlResultFactory Original { set; }
        Type ModelType { get; }
    }

    public interface IGraphQlAccepts<T> : IGraphQlAccepts
    {
        new IGraphQlResultFactory<T> Original { set; }

        IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        Type IGraphQlAccepts.ModelType => typeof(T);
    }
}