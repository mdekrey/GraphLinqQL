using System.Linq.Expressions;

namespace GraphLinqQL
{
    public interface IGraphQlJoin
    {
        ParameterExpression Placeholder { get; }
        LambdaExpression Conversion { get; }
    }

}
