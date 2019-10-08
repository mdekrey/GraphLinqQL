using System.Collections.Generic;

namespace GraphLinqQL.ErrorMessages
{
    public interface IMessageProvider
    {
        bool Supports(string errorCode);
        string GetMessage(string errorCode, IDictionary<string, object> arguments);
    }
}