using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Execution
{
    class ValueConverter : IValueConverter
    {
        public object? Visit(IValueNode value, ValueConverterContext converterContext, Type expectedType, bool nullable = true)
        {
            return value.AcceptConverter(this, converterContext, expectedType, nullable);
        }

        public object? VisitArray(ArrayValue arrayValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            var elementType = TypeSystem.GetElementType(expectedType);
            if (elementType == expectedType)
            {
                throw new ArgumentException($"Expected an array type, got {expectedType.FullName}", nameof(expectedType));
            }
            return arrayValue.Values.Select(v => v.AcceptConverter(this, converterContext, expectedType));
        }

        public object? VisitBoolean(BooleanValue booleanValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            if (expectedType == typeof(bool))
            {
                return booleanValue.TokenValue;
            }
            else if (expectedType == typeof(bool?))
            {
                return (bool?)booleanValue.TokenValue;
            }
            else
            {
                return Convert.ChangeType(booleanValue.TokenValue, expectedType, CultureInfo.InvariantCulture);
            }
        }

        public object? VisitEnum(EnumValue enumValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            if (expectedType.IsEnum)
            {
                return Enum.Parse(expectedType, enumValue.TokenValue);
            }
            // TODO - nullable enum
            else
            {
                return Convert.ChangeType(enumValue.TokenValue, expectedType, CultureInfo.InvariantCulture);
            }
        }

        public object? VisitFloat(FloatValue floatValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            if (expectedType == typeof(double))
            {
                return double.Parse(floatValue.TokenValue, CultureInfo.InvariantCulture);
            }
            else if (expectedType == typeof(double?))
            {
                return (double?)double.Parse(floatValue.TokenValue, CultureInfo.InvariantCulture);
            }
            else
            {
                return Convert.ChangeType(double.Parse(floatValue.TokenValue, CultureInfo.InvariantCulture), expectedType, CultureInfo.InvariantCulture);
            }
        }

        public object? VisitInt(IntValue intValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            if (expectedType == typeof(int))
            {
                return int.Parse(intValue.TokenValue, CultureInfo.InvariantCulture);
            }
            else if (expectedType == typeof(int?))
            {
                return (int?)int.Parse(intValue.TokenValue, CultureInfo.InvariantCulture);
            }
            else
            {
                return Convert.ChangeType(int.Parse(intValue.TokenValue, CultureInfo.InvariantCulture), expectedType, CultureInfo.InvariantCulture);
            }
        }

        public object? VisitNull(NullValue nullValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            return null;
        }

        public object? VisitObject(ObjectValue objectValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            throw new NotImplementedException();
        }

        public object? VisitString(IStringValue stringValue, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            if (expectedType == typeof(string))
            {
                return stringValue.Text;
            }
            else
            {
                return Convert.ChangeType(stringValue.Text, expectedType, CultureInfo.InvariantCulture);
            }
        }

        public object? VisitVariable(Variable variable, ValueConverterContext converterContext, Type expectedType, bool nullable)
        {
            var result = converterContext.GetVariableValues(variable.Name, expectedType);
            if (result == null && !nullable)
            {
                throw new ArgumentNullException(nameof(variable));
            }
            // TODO - maybe should type check this further?
            return result;
        }
    }
}
