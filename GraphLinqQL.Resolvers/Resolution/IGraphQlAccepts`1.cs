﻿using System;
using System.Linq;

namespace GraphLinqQL.Resolution
{
    public interface IGraphQlAccepts<in T> : IGraphQlAccepts
    {
    }
}