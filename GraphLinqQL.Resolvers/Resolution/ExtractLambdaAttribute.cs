using System;

namespace GraphLinqQL.Resolution
{
    [System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ExtractLambdaAttribute : Attribute
    {
    }
}
