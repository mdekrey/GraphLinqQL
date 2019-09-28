﻿namespace GraphLinqQL.Ast.Nodes
{
    public enum NodeKind
    {
        Document,
        OperationDefinition,
        SelectionSet,
        VariableDefinition,
        Variable,
        Field,
        Directive,
        Argument,
        TypeName,
        ArrayValue,
        StringValue,
        IntValue,
        TripleQuotedStringValue,
        FloatValue,
        BooleanValue,
        NullValue,
        EnumValue,
        ListType,
        NonNullType,
    }
}