using Newtonsoft.Json.Linq;
using Snapper;
using Snapper.Attributes;
using System;
using System.Collections.Generic;
using Xunit;
using Interfaces = GraphLinqQL.HandwrittenSamples.Interfaces;
using Implementations = GraphLinqQL.HandwrittenSamples.Implementations;
using System.Collections.Immutable;
using GraphLinqQL.Execution;
using GraphLinqQL.Stubs;

namespace GraphLinqQL.Execution
{
    public class GraphQlExecutorShould
    {
        private class GraphQlExecutionOptions : IGraphQlExecutionOptions
        {
            public Type? Query => typeof(Implementations.QueryContract);

            public Type? Mutation => null;

            public Type? Subscription => null;

            public IReadOnlyList<IGraphQlDirective> Directives { get; } = new IGraphQlDirective[] { new Directives.SkipDirective(), new Directives.IncludeDirective() };

            public IGraphQlTypeResolver TypeResolver { get; } = new Interfaces.TypeResolver();
        }

        private static IGraphQlExecutor CreateExecutor()
        {
            using var serviceProvider = new SimpleServiceProvider();
            return new GraphQlExecutor(serviceProvider, new GraphQlExecutionOptions());
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  hero {
    id
    name
  }
  rand
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToRepresentUntypedSimpleListStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
  }
  rand
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToRepresentNestedStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    friends {
      id
      name
    }
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToUseStructureFragments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
fragment HeroPrimary on Hero {
  id
  name
}

{
  heroes {
    ...HeroPrimary
    friends {
      ...HeroPrimary
    }
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToRepresentComplexStructures()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    renown
    faction
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToPassParameters()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    location
    oldLocation: location(date: ""2008-05-02"")
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToPassParametersWithNonStringTypes()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes(first: 1) {
    id
    name
    renown
    faction
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToPassArguments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
query Heroes($date: String!) {
  heroes {
    id
    name
    location(date: $date)
  }
}
", new Dictionary<string, string> { { "date", "\"2008-05-02\"" } });

            result.ShouldMatchSnapshot();
        }

        [Fact(Skip = "GraphQL-Parse does not support query default parameters")]
        public void BeAbleToPassArgumentsWithDefaultValues()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
query Heroes($date: String = ""2019-04-22"", $date2 = ""2012-05-04"") {
  heroes {
    id
    name
    location(date: $date)
    avengersLocation: location(date: $date2)
  }
}
", new Dictionary<string, string> { { "date", "\"2008-05-02\"" } });

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToUseDirectives()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    renown @include(if: false)
    faction @skip(if: true)
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToUseInlineFragments()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    ... {
      renown
      faction
    }
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToUseInlineFragmentsWithTypeConditions()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  heroes {
    id
    name
    ... on Hero {
      renown
      faction
    }
    ... on NonHero {
      crash
    }
  }
}
");

            result.ShouldMatchSnapshot();
        }


        [Fact]
        public void BeAbleToUseInlineFragmentsWithTypeConditionsOnUnions()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  characters {
    id
    name
    ... on Hero {
      renown
      faction
    }
    ... on Villain {
      goal
    }
  }
}
");

            result.ShouldMatchSnapshot();
        }

        [Fact]
        public void BeAbleToGetTypenames()
        {
            using var executor = CreateExecutor();
            var result = executor.Execute(@"
{
  characters {
    id
    name
    __typename
  }
}
");

            result.ShouldMatchSnapshot();
        }
    }

}
