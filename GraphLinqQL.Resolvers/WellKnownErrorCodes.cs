using GraphLinqQL.ErrorMessages;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace GraphLinqQL
{
    using MessageResolver = Func<IDictionary<string, object>, string>;

    public class WellKnownErrorCodes : IMessageProvider
    {
        public const string NoOperation = "noOperationFound";
        public const string RequiredSubselection = "requiredSubselection";
        public const string NoSubselectionAllowed = "noSubselectionAllowed";
        public const string UndefinedField = "undefinedField";
        public const string ParseError = "parseError";

        private static readonly IReadOnlyDictionary<string, MessageResolver> messages = ConstructMessages();

        private static IReadOnlyDictionary<string, MessageResolver> ConstructMessages()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, MessageResolver>();
            builder.Add(NoOperation, args => ErrorMessages.ErrorMessages.noOperationFound);
            builder.Add(RequiredSubselection, args => string.Format(CultureInfo.InvariantCulture, ErrorMessages.ErrorMessages.requiredSubselection, args["fieldName"], args["type"]));
            builder.Add(NoSubselectionAllowed, args => string.Format(CultureInfo.InvariantCulture, ErrorMessages.ErrorMessages.noSubselectionAllowed, args["fieldName"], args["type"]));
            builder.Add(UndefinedField, args => string.Format(CultureInfo.InvariantCulture, ErrorMessages.ErrorMessages.undefinedField, args["fieldName"], args["type"]));
            builder.Add(ParseError, args => string.Format(CultureInfo.InvariantCulture, ErrorMessages.ErrorMessages.parseError, args["innerMessage"]));
            return builder.ToImmutable();
        }

        public string GetMessage(string errorCode, IDictionary<string, object> arguments)
        {
            return messages[errorCode](arguments);
        }

        public bool Supports(string errorCode)
        {
            return messages.ContainsKey(errorCode);
        }
    }
}
