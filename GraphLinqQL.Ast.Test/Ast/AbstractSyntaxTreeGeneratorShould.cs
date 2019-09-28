using GraphLinqQL.Ast;
using GraphLinqQL.Ast.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
            var actual = JsonConvert.SerializeObject(document, JsonSettings());
            var expected = "{\"Kind\":\"Document\",\"Children\":[{\"Kind\":\"OperationDefinition\",\"OperationType\":\"Query\",\"Name\":null,\"Variables\":[],\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"hero\",\"Alias\":null,\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"id\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"name\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]}]},\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"rand\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]}]}}]}";

            Assert.Equal(expected, actual);
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
            var actual = JsonConvert.SerializeObject(document, JsonSettings());
            var expected = "{\"Kind\":\"Document\",\"Children\":[{\"Kind\":\"OperationDefinition\",\"OperationType\":\"Query\",\"Name\":\"Heroes\",\"Variables\":[{\"Kind\":\"VariableDefinition\",\"Variable\":{\"Kind\":\"Variable\",\"Name\":\"date\"},\"GraphQLType\":{\"Kind\":\"TypeName\",\"Name\":\"String\"},\"DefaultValueJson\":null},{\"Kind\":\"VariableDefinition\",\"Variable\":{\"Kind\":\"Variable\",\"Name\":\"date2\"},\"GraphQLType\":{\"Kind\":\"TypeName\",\"Name\":\"String\"},\"DefaultValueJson\":null}],\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"heroes\",\"Alias\":null,\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"id\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"name\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"location\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[{\"Kind\":\"Argument\",\"Name\":\"date\",\"Value\":{\"Kind\":\"Variable\",\"Name\":\"date\"}}],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"location\",\"Alias\":\"avengersLocation:\",\"SelectionSet\":null,\"Arguments\":[{\"Kind\":\"Argument\",\"Name\":\"date\",\"Value\":{\"Kind\":\"Variable\",\"Name\":\"date2\"}}],\"Directives\":[]}]},\"Arguments\":[],\"Directives\":[]}]}}]}";

            Assert.Equal(expected, actual);
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
            var actual = JsonConvert.SerializeObject(document, JsonSettings());
            var expected = "{\"Kind\":\"Document\",\"Children\":[{\"Kind\":\"OperationDefinition\",\"OperationType\":\"Query\",\"Name\":\"Heroes\",\"Variables\":[{\"Kind\":\"VariableDefinition\",\"Variable\":{\"Kind\":\"Variable\",\"Name\":\"date\"},\"GraphQLType\":{\"Kind\":\"ListType\",\"ElementType\":{\"Kind\":\"NonNullType\",\"BaseType\":{\"Kind\":\"TypeName\",\"Name\":\"String\"}}},\"DefaultValueJson\":null},{\"Kind\":\"VariableDefinition\",\"Variable\":{\"Kind\":\"Variable\",\"Name\":\"date2\"},\"GraphQLType\":{\"Kind\":\"NonNullType\",\"BaseType\":{\"Kind\":\"TypeName\",\"Name\":\"String\"}},\"DefaultValueJson\":null}],\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"heroes\",\"Alias\":null,\"SelectionSet\":{\"Kind\":\"SelectionSet\",\"Selections\":[{\"Kind\":\"Field\",\"Name\":\"id\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"name\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"locations\",\"Alias\":null,\"SelectionSet\":null,\"Arguments\":[{\"Kind\":\"Argument\",\"Name\":\"dates\",\"Value\":{\"Kind\":\"Variable\",\"Name\":\"date\"}}],\"Directives\":[]},{\"Kind\":\"Field\",\"Name\":\"locations\",\"Alias\":\"locations2:\",\"SelectionSet\":null,\"Arguments\":[{\"Kind\":\"Argument\",\"Name\":\"dates\",\"Value\":{\"Kind\":\"ArrayValue\",\"Values\":[{\"Kind\":\"Variable\",\"Name\":\"date2\"}]}}],\"Directives\":[]}]},\"Arguments\":[],\"Directives\":[]}]}}]}";

            Assert.Equal(expected, actual);
        }

        private JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new PropertyIgnoreSerializerContractResolver(),
                Converters = { new StringEnumConverter() }
            };
        }
    }

    class PropertyIgnoreSerializerContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == nameof(INode.Location))
            {
                return null!;
            }
            return property;
        }
    }

}
