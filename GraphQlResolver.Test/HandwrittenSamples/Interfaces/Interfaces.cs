using GraphQlResolver.CommonTypes;
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

    public class TypeResolver : IGraphQlTypeResolver
    {
        public Type Resolve(string name)
        {
            switch (name)
            {
                case "ID":
                    return typeof(int?);
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
        public abstract IGraphQlResult<IEnumerable<Hero>> Heroes();
        public abstract IGraphQlResult<double> Rand();

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IDictionary<string, object> parameters) =>
            name switch
            {
                "heroes" => Heroes(),
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
            Type IGraphQlAccepts.ModelType => typeof(T);
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
        public abstract IGraphQlResult<string> Location(string date);

        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, IDictionary<string, object> parameters) =>
            name switch
            {
                "id" => Id(),
                "name" => Name(),
                "renown" => Renown(),
                "faction" => Faction(),
                "friends" => Friends(),
                "location" => Location(date: (parameters.TryGetValue("date", out var date) ? date as string : null) ?? "2019-04-22"),
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
            Type IGraphQlAccepts.ModelType => typeof(T);
        }
    }
}
