using GraphLinqQL.CommonTypes;
using GraphLinqQL.Introspection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CA1033 // Interface methods should be callable by child types
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1716 // Identifiers should not match keywords
#pragma warning disable CA1724 // Name conflicts in whole or in part with a namespace

namespace GraphLinqQL.HandwrittenSamples.Interfaces
{
    //type Villain {
    //  id: ID!
    //  name: String!
    //  goal: String!
    //}
    //type Hero {
    //  id: ID!
    //  name: String!
    //  renown: Float!
    //  faction: String!
    //  friends: [Hero!]!
    //  location(date: String = "2019-04-22"): String!;
    //}
    //union Character = Hero | Villain
    //type Query {
    //  characters: [Character!]!
    //  heroes(first: Int): [Hero!]!
    //  nulls: [Hero!]
    //  nohero: Hero
    //  hero: Hero!
    //  heroFinalized: Hero!
    //  heroById(id: String!): Hero
    //  rand: Float!
    //}
    //schema {
    //  query: Query
    //}

    public class TypeResolver : IGraphQlTypeResolver
    {
        public Type Resolve(string name)
        {
            switch (name)
            {
                case "ID":
                    return typeof(string);
                case "Int":
                    return typeof(int?);
                case "Float":
                    return typeof(double?);
                case "String":
                    return typeof(string);
                case "Boolean":
                    return typeof(bool?);
                default:
                    throw new ArgumentException("Unknown type " + name, nameof(name));
            }
        }
    }

    public abstract class Query : IGraphQlResolvable
    {
        private Query() { }
        public abstract IGraphQlResult<IEnumerable<Hero>> Heroes(int? first);
        public abstract IGraphQlResult<IEnumerable<Hero>?> Nulls();
        public abstract IGraphQlResult<Hero> Hero();
        public abstract IGraphQlResult<Hero> HeroFinalized();
        public abstract IGraphQlResult<Hero> HeroById(string id);
        public abstract IGraphQlResult<Hero?> NoHero();
        public abstract IGraphQlResult<double> Rand();
        public abstract IGraphQlResult<IEnumerable?> Characters();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__typename" => GraphQlConstantResult.Construct("Query"),
                "characters" => Characters(),
                "heroes" => Heroes(first: (parameters.HasParameter("first") ? parameters.GetParameter<int?>("first") : null)),
                "nulls" => Nulls(),
                "nohero" => NoHero(),
                "hero" => Hero(),
                "heroFinalized" => HeroFinalized(),
                "heroById" => HeroById(id: (parameters.GetParameter<string>("id"))),
                "rand" => Rand(),
                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
            };

        bool IGraphQlResolvable.IsType(string value) =>
            value == "Query";

        public abstract class GraphQlContract<T> : Query, IGraphQlAccepts<T>
        {
#nullable disable
            public IGraphQlResultFactory<T> Original { get; set; }
#nullable restore

            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }

    public abstract class Hero : IGraphQlResolvable
    {
        internal Hero() { }
        public abstract IGraphQlResult<GraphQlId> Id();
        public abstract IGraphQlResult<string> Name();
        public abstract IGraphQlResult<double> Renown();
        public abstract IGraphQlResult<string> Faction();
        public abstract IGraphQlResult<IEnumerable<Hero>> Friends();
        public abstract IGraphQlResult<IEnumerable<Hero>> FriendsDeferred();
        public abstract IGraphQlResult<string> Location(string date);

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__typename" => GraphQlConstantResult.Construct("Hero"),
                "id" => Id(),
                "name" => Name(),
                "renown" => Renown(),
                "faction" => Faction(),
                "friends" => Friends(),
                "friendsDeferred" => FriendsDeferred(),
                "location" => Location(date: (parameters.HasParameter("date") ? parameters.GetParameter<string>("date") : null) ?? "2019-04-22"),
                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
            };

        bool IGraphQlResolvable.IsType(string value) =>
            value == "Hero";

        public abstract class GraphQlContract<T> : Hero, IGraphQlAccepts<T>
        {
#nullable disable
            public IGraphQlResultFactory<T> Original { get; set; }
#nullable restore

            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }


    public abstract class Villain : IGraphQlResolvable
    {
        internal Villain() { }
        public abstract IGraphQlResult<GraphQlId> Id();
        public abstract IGraphQlResult<string> Name();
        public abstract IGraphQlResult<string> Goal();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IGraphQlParameterResolver parameters) =>
            name switch
            {
                "__typename" => GraphQlConstantResult.Construct("Villain"),
                "id" => Id(),
                "name" => Name(),
                "goal" => Goal(),
                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
            };

        bool IGraphQlResolvable.IsType(string value) =>
            value == "Villain";

        public abstract class GraphQlContract<T> : Villain, IGraphQlAccepts<T>
        {
#nullable disable
            public IGraphQlResultFactory<T> Original { get; set; }
#nullable restore

            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
        }
    }
}
