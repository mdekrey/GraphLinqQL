using GraphLinqQL.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GraphLinqQL.Execution
{
    class ValueConverter : IValueVisitor<object?, Type>
    {
        private readonly GraphQLExecutionContext executionContext;

        public ValueConverter(GraphQLExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public object? Visit(IValueNode value, Type expectedType)
        {
            return value.AcceptConverter(this, expectedType);
        }

        public object? VisitArray(ArrayValue arrayValue, Type expectedType)
        {
            var elementType = TypeSystem.GetElementType(expectedType);
            if (elementType == expectedType)
            {
                throw new ArgumentException($"Expected an array type, got {expectedType.FullName}", nameof(expectedType));
            }
            return arrayValue.Values.Select(v => Visit(v, expectedType)).ToArray();
        }

        public object? VisitBoolean(BooleanValue booleanValue, Type expectedType)
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

        public object? VisitEnum(EnumValue enumValue, Type expectedType)
        {
            if (expectedType.IsEnum)
            {
                return TypeDescriptor.GetConverter(expectedType).ConvertTo(enumValue.TokenValue, expectedType);
            }
            else if (expectedType.IsConstructedGenericType && expectedType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var t = Nullable.GetUnderlyingType(expectedType);
                if (t.IsEnum)
                {
                    return Activator.CreateInstance(expectedType, TypeDescriptor.GetConverter(t).ConvertTo(enumValue.TokenValue, t));
                }
            }
            return Convert.ChangeType(enumValue.TokenValue, expectedType, CultureInfo.InvariantCulture);
        }

        public object? VisitFloat(FloatValue floatValue, Type expectedType)
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

        public object? VisitInt(IntValue intValue, Type expectedType)
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

        public object? VisitNull(NullValue nullValue, Type expectedType)
        {
            return null;
        }

        public object? VisitObject(ObjectValue objectValue, Type expectedType)
        {
            var result = (IInputType)Activator.CreateInstance(expectedType);
            foreach (var field in objectValue.Fields)
            {
                result.SetValue(field.Key, type => Visit(field.Value, type));
            }
            return result;
        }

        public object? VisitString(IStringValue stringValue, Type expectedType)
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

        public object? VisitVariable(Variable variable, Type expectedType)
        {
            return executionContext.Arguments[variable.Name].BindTo(expectedType);
        }
    }
}
