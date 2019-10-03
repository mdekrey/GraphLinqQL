using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphLinqQL.Ast.Nodes
{
    public class StringValue : NodeBase, IValueNode, IStringValue
    {
        public StringValue(string quotedStringValue, LocationRange location) : base(location)
        {
            QuotedStringValue = quotedStringValue;
        }

        public override NodeKind Kind => NodeKind.StringValue;

        public string QuotedStringValue { get; }

        public string Text => Unescape(QuotedStringValue.Substring(1, QuotedStringValue.Length - 2));


        private static char BackSlash = '\\';
        private static char Quote = '"';
        private static char LineFeed = '\n';
        private static char CarriageReturn = '\r';
        private static char Slash = '/';
        private static char Tab = '\t';
        private static char BackSpace = '\b';
        private static char FormFeed = '\f';
        private static string Unescape(string source)
        {
            // This code is adapted from https://github.com/dotnet/corefx/blob/82408cd90f4d4573d502e8df2ca437b35e6a37f7/src/System.Text.Json/src/System/Text/Json/Reader/JsonReaderHelper.Unescaping.cs
            // but we don't have spans here
            // TODO - add a .Net 2.1 target and use spans
            var destination = new StringBuilder();

            for (var idx = 0; idx < source.Length; idx++)
            {
                char currentByte = source[idx];
                if (currentByte == BackSlash)
                {
                    idx++;
                    currentByte = source[idx];

                    if (currentByte == Quote)
                    {
                        destination.Append(Quote);
                    }
                    else if (currentByte == 'n')
                    {
                        destination.Append(LineFeed);
                    }
                    else if (currentByte == 'r')
                    {
                        destination.Append(CarriageReturn);
                    }
                    else if (currentByte == BackSlash)
                    {
                        destination.Append(BackSlash);
                    }
                    else if (currentByte == Slash)
                    {
                        destination.Append(Slash);
                    }
                    else if (currentByte == 't')
                    {
                        destination.Append(Tab);
                    }
                    else if (currentByte == 'b')
                    {
                        destination.Append(BackSpace);
                    }
                    else if (currentByte == 'f')
                    {
                        destination.Append(FormFeed);
                    }
                    else if (currentByte == 'u')
                    {
                        // The source is known to be valid JSON, and hence if we see a \u, it is guaranteed to have 4 hex digits following it
                        // Otherwise, the Utf8JsonReader would have alreayd thrown an exception.
                        Debug.Assert(source.Length >= idx + 5);

                        var result = uint.TryParse(source.Substring(idx + 1, 4), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var scalar);
                        Debug.Assert(result);
                        idx += 4;     // The loop iteration will increment idx past the last hex digit

                        //if (JsonHelpers.IsInRangeInclusive((uint)scalar, JsonConstants.HighSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                        //{
                        //    // The first hex value cannot be a low surrogate.
                        //    if (scalar >= JsonConstants.LowSurrogateStartValue)
                        //    {
                        //        ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16(scalar);
                        //    }

                        //    Debug.Assert(JsonHelpers.IsInRangeInclusive((uint)scalar, JsonConstants.HighSurrogateStartValue, JsonConstants.HighSurrogateEndValue));

                        //    idx += 3;   // Skip the last hex digit and the next \u

                        //    // We must have a low surrogate following a high surrogate.
                        //    if (source.Length < idx + 4 || source[idx - 2] != '\\' || source[idx - 1] != 'u')
                        //    {
                        //        ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16();
                        //    }

                        //    // The source is known to be valid JSON, and hence if we see a \u, it is guaranteed to have 4 hex digits following it
                        //    // Otherwise, the Utf8JsonReader would have alreayd thrown an exception.
                        //    result = Utf8Parser.TryParse(source.Slice(idx, 4), out int lowSurrogate, out bytesConsumed, 'x');
                        //    Debug.Assert(result);
                        //    Debug.Assert(bytesConsumed == 4);

                        //    // If the first hex value is a high surrogate, the next one must be a low surrogate.
                        //    if (!JsonHelpers.IsInRangeInclusive((uint)lowSurrogate, JsonConstants.LowSurrogateStartValue, JsonConstants.LowSurrogateEndValue))
                        //    {
                        //        ThrowHelper.ThrowInvalidOperationException_ReadInvalidUTF16(lowSurrogate);
                        //    }

                        //    idx += bytesConsumed - 1;  // The loop iteration will increment idx past the last hex digit

                        //    // To find the unicode scalar:
                        //    // (0x400 * (High surrogate - 0xD800)) + Low surrogate - 0xDC00 + 0x10000
                        //    scalar = (JsonConstants.BitShiftBy10 * (scalar - JsonConstants.HighSurrogateStartValue))
                        //        + (lowSurrogate - JsonConstants.LowSurrogateStartValue)
                        //        + JsonConstants.UnicodePlane01StartValue;
                        //}

                        destination.Append((char)scalar);
                    }
                }
                else
                {
                    destination.Append(currentByte);
                }
            }
            return destination.ToString();
        }

        public TResult AcceptConverter<TResult, TContext>(IValueVisitor<TResult, TContext> converter, TContext context)
        {
            return converter.VisitString((IStringValue)this, context);
        }
    }
}
