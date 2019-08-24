using GraphQlSchema;
using System;

namespace GraphQl.StarWarsSample
{
    public interface Schema
    {
        IResolver<Query> Query();
        IResolver<Mutation> Mutation();
        IResolver<Subscription> Subscription();
    }
}
