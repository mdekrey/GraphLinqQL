﻿using GraphLinqQL.Ast;
using GraphLinqQL.Ast.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Snapper;
using Snapper.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Xunit;

namespace GraphLinqQL.Ast
{
    public class AbstractSyntaxTreeGeneratorShould
    {
        public static IAbstractSyntaxTreeGenerator CreateTarget()
        {
            return new AbstractSyntaxTreeGenerator();
        }

        private void MatchParsedDocumentToSnapshot(string gql)
        {
            var target = CreateTarget();
            var document = target.ParseDocument(gql);
            document.ShouldMatchSnapshot();
        }

        private void ExpectParseError(string gql)
        {
            var target = CreateTarget();
            Assert.Throws<GraphqlParseException>(() => target.ParseDocument(gql));
        }

        [Fact]
        public void ParseFromQuery() => MatchParsedDocumentToSnapshot(@"
{
  hero {
    id
    name
  }
  rand
}
");

        [Fact]
        public void ExpectParametersToHaveTypes() => ExpectParseError(@"
query Heroes($date: String = ""2019-04-22"", $date2 = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
");

        [Fact]
        public void ParseArgumentsWithDefaultValues() => MatchParsedDocumentToSnapshot(@"
query Heroes($date: String = ""2019-04-22"", $date2: String = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
");

        [Fact]
        public void ParseArgumentsWithComplexValues() => MatchParsedDocumentToSnapshot(@"
query Heroes($date: [String!] = ""2019-04-22"", $date2: String! = ""2012-05-04"") {
  heroes {
    id
    name
    locations(dates: $date)
    locations2: locations(dates: [$date2])
  }
}
");

        [Fact]
        public void ParseArgumentsWithObjects() => MatchParsedDocumentToSnapshot(@"
mutation CreateReviewForEpisode {
  createReview(episode: JEDI, review: {
    stars: 5,
    commentary: ""This is a great movie!""
  }) {
    stars
    commentary
  }
}
");

        [Fact]
        public void ParseArgumentsWithObjectsAndVariables() => MatchParsedDocumentToSnapshot(@"
mutation CreateReviewForEpisode($ep: Episode!, $stars: Int!, $commentary: String!) {
  createReview(episode: $ep, review: {
    stars: $stars,
    commentary: $commentary
  }) {
    stars
    commentary
  }
}
");

        // These queries were originally from https://graphql.org/learn/queries/
        [Fact]
        public void IgnoreComments() => MatchParsedDocumentToSnapshot(@"
{
  hero {
    name
    # Queries can have comments!
    friends {
      name
    }
  }
}
");

        [Fact]
        public void AllowArguments() => MatchParsedDocumentToSnapshot(@"
{
  human(id: ""1000"") {
    name
    height
  }
}
");

        [Fact]
        public void AllowNestedArguments() => MatchParsedDocumentToSnapshot(@"
{
  human(id: ""1000"") {
    name
    height(unit: FOOT)
  }
}
");

        [Fact]
        public void AllowAliases() => MatchParsedDocumentToSnapshot(@"
{
  empireHero: hero(episode: EMPIRE) {
    name
  }
  jediHero: hero(episode: JEDI) {
    name
  }
}
");

        [Fact]
        public void AllowFragments() => MatchParsedDocumentToSnapshot(@"
{
  leftComparison: hero(episode: EMPIRE) {
    ...comparisonFields
  }
  rightComparison: hero(episode: JEDI) {
    ...comparisonFields
  }
}

fragment comparisonFields on Character {
  name
  appearsIn
  friends {
    name
  }
}
");

        [Fact]
        public void AllowFragmentsWithVariables() => MatchParsedDocumentToSnapshot(@"
query HeroComparison($first: Int = 3) {
  leftComparison: hero(episode: EMPIRE) {
    ...comparisonFields
  }
  rightComparison: hero(episode: JEDI) {
    ...comparisonFields
  }
}

fragment comparisonFields on Character {
  name
  friendsConnection(first: $first) {
    totalCount
    edges {
      node {
        name
      }
    }
  }
}
");

        [Fact]
        public void AllowOperationNames() => MatchParsedDocumentToSnapshot(@"
query HeroNameAndFriends {
  hero {
    name
    friends {
      name
    }
  }
}
");

        [Fact]
        public void AllowVariables() => MatchParsedDocumentToSnapshot(@"
query HeroNameAndFriends($episode: Episode) {
  hero(episode: $episode) {
    name
    friends {
      name
    }
  }
}
");

        [Fact]
        public void AllowVariablesWithDefaults() => MatchParsedDocumentToSnapshot(@"
query HeroNameAndFriends($episode: Episode = JEDI) {
  hero(episode: $episode) {
    name
    friends {
      name
    }
  }
}
");

        [Fact]
        public void AllowDirectives() => MatchParsedDocumentToSnapshot(@"
query Hero($episode: Episode, $withFriends: Boolean!) {
  hero(episode: $episode) {
    name
    friends @include(if: $withFriends) {
      name
    }
  }
}
");

        [Fact]
        public void AllowMutations() => MatchParsedDocumentToSnapshot(@"
mutation CreateReviewForEpisode($ep: Episode!, $review: ReviewInput!) {
  createReview(episode: $ep, review: $review) {
    stars
    commentary
  }
}
");

        [Fact]
        public void AllowInlineFragments() => MatchParsedDocumentToSnapshot(@"
query HeroForEpisode($ep: Episode!) {
  hero(episode: $ep) {
    name
    ... on Droid {
      primaryFunction
    }
    ... on Human {
      height
    }
  }
}
");

        [Fact]
        public void AllowMetaFields() => MatchParsedDocumentToSnapshot(@"
{
  search(text: ""an"") {
    __typename
    ... on Human {
      name
    }
    ... on Droid {
      name
    }
    ... on Starship {
      name
    }
  }
}
");

        // These queries were originally from https://graphql.org/learn/schema/
        [Fact]
        public void AllowTypeDefinitions() => MatchParsedDocumentToSnapshot(@"
type Character {
  name: String!
  appearsIn: [Episode!]!
}
");

        [Fact]
        public void AllowTypeDefinitionsWithArguments() => MatchParsedDocumentToSnapshot(@"
type Starship {
  id: ID!
  name: String!
  length(unit: LengthUnit = METER): Float
}
");

        [Fact]
        public void AllowSchemaDefinition() => MatchParsedDocumentToSnapshot(@"
schema {
  query: Query
  mutation: Mutation
}
");

        [Fact]
        public void AllowArgumentStrings() => MatchParsedDocumentToSnapshot(@"
query {
  hero {
    name
  }
  droid(id: ""2000"") {
    name
  }
}
");

        [Fact]
        public void AllowQueryTypes() => MatchParsedDocumentToSnapshot(@"
type Query {
  hero(episode: Episode): Character
  droid(id: ID!): Droid
}
");

        [Fact]
        public void AllowCustomScalarTypes() => MatchParsedDocumentToSnapshot(@"
scalar Date
");

        [Fact]
        public void AllowEnumTypes() => MatchParsedDocumentToSnapshot(@"
enum Episode {
  NEWHOPE
  EMPIRE
  JEDI
}
");

        [Fact]
        public void AllowListAndNonNullFields() => MatchParsedDocumentToSnapshot(@"
type Character {
  name: String!
  appearsIn: [Episode]!
}
");

        [Fact]
        public void AllowNonNullArguments() => MatchParsedDocumentToSnapshot(@"
query DroidById($id: ID!) {
  droid(id: $id) {
    name
  }
}
");

        [Fact]
        public void AllowInterfaces() => MatchParsedDocumentToSnapshot(@"
interface Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
}
");

        [Fact]
        public void AllowInterfaceImplementations() => MatchParsedDocumentToSnapshot(@"
type Human implements Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
  starships: [Starship]
  totalCredits: Int
}

type Droid implements Character {
  id: ID!
  name: String!
  friends: [Character]
  appearsIn: [Episode]!
  primaryFunction: String
}

");

        [Fact]
        public void AllowInlineFragmentsWithTypeConditions() => MatchParsedDocumentToSnapshot(@"
query HeroForEpisode($ep: Episode!) {
  hero(episode: $ep) {
    name
    ... on Droid {
      primaryFunction
    }
  }
}
");

        [Fact]
        public void AllowUnionTypes() => MatchParsedDocumentToSnapshot(@"
union SearchResult = Human | Droid | Starship
");

        [Fact]
        public void AllowMultipleInlineFragments() => MatchParsedDocumentToSnapshot(@"
{
  search(text: ""an"") {
    __typename
    ... on Character {
      name
    }
    ... on Human {
      height
    }
    ... on Droid {
      primaryFunction
    }
    ... on Starship {
      name
      length
    }
  }
}
");

        [Fact]
        public void AllowInputTypes() => MatchParsedDocumentToSnapshot(@"
input ReviewInput {
  stars: Int!
  commentary: String
}
");
    }
}
