using GraphQlSchema;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlResolver.HandwrittenSamples.Interfaces
{
    //type Hero {
    //  id: ID!
    //  name: String!
    //  renown: Float!
    //  faction: String!
    //  friends: [Hero!]!
    //  location(date: String = "2019-04-22"): String!;
    //}
    //type Query {
    //  heroes: [Hero!]!
    //}
    //schema {
    //  query: Query
    //}

    public interface Query : IGraphQlResolvable
    {
        IGraphQlResult<IEnumerable<Hero>> Heroes();
        IGraphQlResult<double> Rand();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
            name switch
            {
                "heroes" => Heroes(),
                "rand" => Rand(),
                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
            };
    }

    public interface Hero : IGraphQlResolvable
    {
        IGraphQlResult<GraphQlId> Id();
        IGraphQlResult<string> Name();
        IGraphQlResult<double> Renown();
        IGraphQlResult<string> Faction();
        IGraphQlResult<IEnumerable<Hero>> Friends();
        IGraphQlResult<string> Location(string date);

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, params object[] parameters) =>
            name switch
            {
                "id" => Id(),
                "name" => Name(),
                "renown" => Renown(),
                "faction" => Faction(),
                "friends" => Friends(),
                "location" => Location((string)parameters[0] ?? "2019-04-22"),
                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
            };
    }
}
