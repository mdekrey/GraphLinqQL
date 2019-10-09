//using GraphLinqQL.Introspection;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//#pragma warning disable CA1033 // Interface methods should be callable by child types
//#pragma warning disable CA1034 // Nested types should not be visible
//#pragma warning disable CA1716 // Identifiers should not match keywords
//#pragma warning disable CA1724 // Name conflicts in whole or in part with a namespace

//namespace GraphLinqQL.HandwrittenSamples.Interfaces
//{
//    //type Villain {
//    //  id: ID!
//    //  name: String!
//    //  goal: String!
//    //}
//    //type Hero {
//    //  id: ID!
//    //  name: String!
//    //  renown: Float!
//    //  faction: String!
//    //  friends: [Hero!]!
//    //  location(date: String = "2019-04-22"): String!;
//    //}
//    //union Character = Hero | Villain
//    //type Query {
//    //  characters: [Character!]!
//    //  heroes(first: Int): [Hero!]!
//    //  nulls: [Hero!]
//    //  nohero: Hero
//    //  hero: Hero!
//    //  heroFinalized: Hero!
//    //  heroById(id: String!): Hero
//    //  rand: Float!
//    //}
//    //schema {
//    //  query: Query
//    //}

//    public class TypeResolver : IGraphQlTypeResolver
//    {
//        public Type IntrospectionTypeListing => typeof(Introspection.TypeListing);

//        public Type ResolveForInput(string name)
//        {
//            switch (name)
//            {
//                case "ID":
//                    return typeof(string);
//                case "Int":
//                    return typeof(int?);
//                case "Float":
//                    return typeof(double?);
//                case "String":
//                    return typeof(string);
//                case "Boolean":
//                    return typeof(bool?);
//                default:
//                    throw new ArgumentException("Unknown type " + name, nameof(name));
//            }
//        }
//    }

//    public abstract class Query : IGraphQlResolvable
//    {
//        private Query() { }
//        public abstract IGraphQlResult<IEnumerable<Hero>> Heroes(FieldContext fieldContext, int? first);
//        public abstract IGraphQlResult<IEnumerable<Hero>?> Nulls(FieldContext fieldContext);
//        public abstract IGraphQlResult<Hero> Hero(FieldContext fieldContext);
//        public abstract IGraphQlResult<Hero> HeroFinalized(FieldContext fieldContext);
//        public abstract IGraphQlResult<Hero> HeroById(FieldContext fieldContext, string id);
//        public abstract IGraphQlResult<Hero?> NoHero(FieldContext fieldContext);
//        public abstract IGraphQlResult<double> Rand(FieldContext fieldContext);
//        public abstract IGraphQlResult<IEnumerable?> Characters(FieldContext fieldContext);

//        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, FieldContext fieldContext, IGraphQlParameterResolver parameters) =>
//            name switch
//            {
//                "__typename" => GraphQlConstantResult.Construct("Query"),
//                "characters" => Characters(fieldContext),
//                "heroes" => Heroes(fieldContext, first: (parameters.HasParameter("first") ? parameters.GetParameter<int?>("first") : null)),
//                "nulls" => Nulls(fieldContext),
//                "nohero" => NoHero(fieldContext),
//                "hero" => Hero(fieldContext),
//                "heroFinalized" => HeroFinalized(fieldContext),
//                "heroById" => HeroById(fieldContext, id: (parameters.GetParameter<string>("id"))),
//                "rand" => Rand(fieldContext),
//                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
//            };

//        bool IGraphQlResolvable.IsType(string value) =>
//            value == "Query";

//        public abstract class GraphQlContract<T> : Query, IGraphQlAccepts<T>
//        {
//#nullable disable
//            public IGraphQlResultFactory<T> Original { get; set; }
//#nullable restore

//            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
//        }
//    }

//    public abstract class Hero : IGraphQlResolvable
//    {
//        internal Hero() { }
//        public abstract IGraphQlResult<string> Id(FieldContext fieldContext);
//        public abstract IGraphQlResult<string> Name(FieldContext fieldContext);
//        public abstract IGraphQlResult<double> Renown(FieldContext fieldContext);
//        public abstract IGraphQlResult<string> Faction(FieldContext fieldContext);
//        public abstract IGraphQlResult<IEnumerable<Hero>> Friends(FieldContext fieldContext);
//        public abstract IGraphQlResult<IEnumerable<Hero>> FriendsDeferred(FieldContext fieldContext);
//        public abstract IGraphQlResult<string> Location(FieldContext fieldContext, string date);

//        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, FieldContext fieldContext, IGraphQlParameterResolver parameters) =>
//            name switch
//            {
//                "__typename" => GraphQlConstantResult.Construct("Hero"),
//                "id" => Id(fieldContext),
//                "name" => Name(fieldContext),
//                "renown" => Renown(fieldContext),
//                "faction" => Faction(fieldContext),
//                "friends" => Friends(fieldContext),
//                "friendsDeferred" => FriendsDeferred(fieldContext),
//                "location" => Location(fieldContext, date: (parameters.HasParameter("date") ? parameters.GetParameter<string>("date") : null) ?? "2019-04-22"),
//                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
//            };

//        bool IGraphQlResolvable.IsType(string value) =>
//            value == "Hero";

//        public abstract class GraphQlContract<T> : Hero, IGraphQlAccepts<T>
//        {
//#nullable disable
//            public IGraphQlResultFactory<T> Original { get; set; }
//#nullable restore

//            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
//        }
//    }


//    public abstract class Villain : IGraphQlResolvable
//    {
//        internal Villain() { }
//        public abstract IGraphQlResult<string> Id(FieldContext fieldContext);
//        public abstract IGraphQlResult<string> Name(FieldContext fieldContext);
//        public abstract IGraphQlResult<string> Goal(FieldContext fieldContext);

//        IGraphQlResult IGraphQlResolvable.ResolveQuery(string name, FieldContext fieldContext, IGraphQlParameterResolver parameters) =>
//            name switch
//            {
//                "__typename" => GraphQlConstantResult.Construct("Villain"),
//                "id" => Id(fieldContext),
//                "name" => Name(fieldContext),
//                "goal" => Goal(fieldContext),
//                _ => throw new ArgumentException("Unknown property " + name, nameof(name))
//            };

//        bool IGraphQlResolvable.IsType(string value) =>
//            value == "Villain";

//        public abstract class GraphQlContract<T> : Villain, IGraphQlAccepts<T>
//        {
//#nullable disable
//            public IGraphQlResultFactory<T> Original { get; set; }
//#nullable restore

//            IGraphQlResultFactory IGraphQlAccepts.Original { set { Original = (IGraphQlResultFactory<T>)value; } }
//        }
//    }

//    namespace Introspection
//    {
//        public class TypeListing : IGraphQlTypeListing
//        {

//            public Type Query => typeof(Query);

//            public Type? Mutation => null;

//            public Type? Subscription => null;

//            public IEnumerable<Type> TypeInformation => throw new NotImplementedException();

//            public IEnumerable<DirectiveInformation> DirectiveInformation => throw new NotImplementedException();

//            public Type? Type(string name)
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}
