namespace GraphLinqQL
{
    public static class GraphQlAcceptsExtension
    {
        public static IGraphQlResultFactory<T> Original<T>(this IGraphQlAccepts<T> accepts)
        {
            return (IGraphQlResultFactory<T>)accepts.Original;
        }
    }
}