using System;
using System.Collections.Generic;
using System.Text;

namespace GraphLinqQL.Resolution
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class InlinableClassAttribute : Attribute
    {
    }
}
