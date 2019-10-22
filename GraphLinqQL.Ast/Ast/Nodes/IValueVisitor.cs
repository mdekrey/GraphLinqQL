namespace GraphLinqQL.Ast.Nodes
{
    public interface IValueVisitor<TResult, TContext>
    {
        TResult Visit(IValueNode value, TContext context);
        TResult VisitObject(ObjectValue objectValue, TContext context);
        TResult VisitArray(ArrayValue arrayValue, TContext context);
        TResult VisitBoolean(BooleanValue booleanValue, TContext context);
        TResult VisitEnum(EnumValue enumValue, TContext context);
        TResult VisitInt(IntValue intValue, TContext context);
        TResult VisitNull(NullValue nullValue, TContext context);
        TResult VisitString(IStringValue stringValue, TContext context);
        TResult VisitVariable(Variable variable, TContext context);
        TResult VisitFloat(FloatValue floatValue, TContext context);
    }
}