using System;
using System.Linq;

namespace GraphLinqQL
{
    public interface IGraphQlAccepts
    {
        IGraphQlResultFactory Original { get; set; }
    }

    public interface IGraphQlAccepts<in T> : IGraphQlAccepts
    {
    }
}