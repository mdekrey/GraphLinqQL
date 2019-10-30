using System;

namespace GraphLinqQL.Resolution
{
    /// <summary>
    /// Use on LambdaExpressions in constructors for use with debugging or otherwise deferred
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class ExtractLambdaAttribute : Attribute
    {
    }
}
