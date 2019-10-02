using System;
using System.Collections.Generic;

namespace GraphLinqQL.Ast.Nodes
{
    public interface IValueNode : INode
    {
        object? AcceptConverter(IValueConverter converter, ValueConverterContext converterContext, Type expectedType, bool nullable = true);
    }

#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly struct ValueConverterContext
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        public ValueConverterContext(Func<string, Type, object?> getVariableValues)
        {
            GetVariableValues = getVariableValues;
        }

        public Func<string, Type, object?> GetVariableValues { get; }
    }

    public interface IValueConverter
    {
        object? Visit(IValueNode value, ValueConverterContext converterContext, Type expectedType, bool nullable = true);
        object? VisitObject(ObjectValue objectValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitArray(ArrayValue arrayValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitBoolean(BooleanValue booleanValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitEnum(EnumValue enumValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitInt(IntValue intValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitNull(NullValue nullValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitString(IStringValue stringValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitVariable(Variable variable, ValueConverterContext converterContext, Type expectedType, bool nullable);
        object? VisitFloat(FloatValue floatValue, ValueConverterContext converterContext, Type expectedType, bool nullable);
    }
}