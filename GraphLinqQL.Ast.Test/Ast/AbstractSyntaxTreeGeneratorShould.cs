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
    [UpdateSnapshots]
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
    }
}
