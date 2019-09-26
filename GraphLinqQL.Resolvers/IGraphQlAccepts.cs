using System;
using System.Linq;

namespace GraphLinqQL
{
    public interface IGraphQlAccepts
    {
        IGraphQlResultFactory Original { set; }
    }

    public interface IGraphQlAccepts<in T> : IGraphQlAccepts
    {
    }
}