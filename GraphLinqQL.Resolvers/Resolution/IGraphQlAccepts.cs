﻿using System;
using System.Linq;

namespace GraphLinqQL.Resolution
{
    public interface IGraphQlAccepts
    {
        IGraphQlResultFactory Original { get; set; }
    }
}