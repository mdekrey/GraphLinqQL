using System;
using System.Linq;

namespace GraphQlResolver
{
    public interface IGraphQlAccepts
    {
        IGraphQlResultFactory Original { set; }
        Type ModelType { get; }
    }

    public interface IGraphQlAccepts<in T> : IGraphQlAccepts
    {
    }
}