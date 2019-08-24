using GraphQlSchema;

namespace GraphQl.StarWarsSample
{
    public interface Subscription
    {
        IResolver<Review> ReviewAdded(Episode? episode);
    }
}