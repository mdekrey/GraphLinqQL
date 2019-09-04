using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver
{
    public interface IGraphQlTypeResolver
    {
        Type Resolve(string name);
    }
}
