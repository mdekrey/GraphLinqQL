using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace GraphLinqQL.ErrorMessages
{
    public class MessageResolver : IMessageResolver
    {
        private readonly ConcurrentDictionary<string, IMessageProvider?> messageProvidersByCode = new ConcurrentDictionary<string, IMessageProvider?>();
        private readonly ImmutableList<IMessageProvider> providers;

        public MessageResolver(IEnumerable<IMessageProvider> messageProviders)
        {
            this.providers = messageProviders.ToImmutableList();
        }

        public string? GetMessage(string errorCode, IDictionary<string, object> arguments)
        {
            return messageProvidersByCode.GetOrAdd(errorCode, FindMessageProvider)
                ?.GetMessage(errorCode, arguments);
        }

        private IMessageProvider? FindMessageProvider(string errorCode)
        {
            return providers.FirstOrDefault(p => p.Supports(errorCode));
        }
    }
}
