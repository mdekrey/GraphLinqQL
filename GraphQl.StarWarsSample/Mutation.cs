using GraphQlSchema;

namespace GraphQl.StarWarsSample
{
    public interface Mutation
    {
        IResolver<Review> CreateReview(Episode? episode, Inputs.ReviewInput review);
    }
}