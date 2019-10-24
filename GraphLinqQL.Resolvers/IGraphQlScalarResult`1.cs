using GraphLinqQL.Resolution;

namespace GraphLinqQL
{
    public interface IGraphQlScalarResult<out TReturnType> : IGraphQlScalarResult, IGraphQlResult
    {
        IGraphQlObjectResult<TContract> AsContract<TContract>()
            where TContract : IGraphQlAccepts<TReturnType>;
    }

}
