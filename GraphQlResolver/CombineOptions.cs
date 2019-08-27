using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQlResolver
{
    public class CombineOptions<T> : Dictionary<string, Func<IQueryable<T>, IQueryable>>
    {
    }
}