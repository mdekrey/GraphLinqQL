using GraphLinqQL.Ast;
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

        [Fact]
        public void ParseFromQuery()
        {
            var target = CreateTarget();
            var document = target.ParseDocument(@"
{
  hero {
    id
    name
  }
  rand
}
");
            document.ShouldMatchSnapshot();
        }

        [Fact]
        public void DetectErrorsInDocuments()
        {
            var target = CreateTarget();
            // $date2 requires a type
            Assert.Throws<GraphqlParseException>(() => target.ParseDocument(@"
query Heroes($date: String = ""2019-04-22"", $date2 = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
"));
        }

        [Fact]
        public void ParseArgumentsWithDefaultValues()
        {
            var target = CreateTarget();
            var document = target.ParseDocument(@"
query Heroes($date: String = ""2019-04-22"", $date2: String = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
");
            document.ShouldMatchSnapshot();
        }

        [Fact]
        public void ParseArgumentsWithComplexValues()
        {
            var target = CreateTarget();
            var document = target.ParseDocument(@"
query Heroes($date: [String!] = ""2019-04-22"", $date2: String! = ""2012-05-04"") {
  heroes {
    id
    name
    locations(dates: $date)
    locations2: locations(dates: [$date2])
  }
}
");
            document.ShouldMatchSnapshot();
        }

        [Fact]
        public void ParseArgumentsWithObjects()
        {
            var target = CreateTarget();
            var document = target.ParseDocument(@"
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
            document.ShouldMatchSnapshot();
        }
    }
}
