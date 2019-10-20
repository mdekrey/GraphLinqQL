using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL
{
    public interface IInputType
    {
        void SetValue(string fieldName, Func<Type, object?> getValue);
    }
}
