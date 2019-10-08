using System.Collections.Generic;

namespace GraphLinqQL.ErrorMessages
{
    public interface IMessageResolver
    {
        string? GetMessage(string errorCode, IDictionary<string, object> arguments);
    }
}