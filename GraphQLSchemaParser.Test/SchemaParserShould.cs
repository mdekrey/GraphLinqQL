using GraphQLParser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace GraphQlSchemaParser.Test
{
    public class SchemaParserShould
    {
        private string StarWarsSchema()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream("GraphQlSchemaParser.Test.TestData.sw-schema.3.graphql"))
            {
                if (stream == null)
                {
                    return null;
                }

                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        [Fact]
        public void BeAbleToParseTheSchema()
        {
            var parser = new Parser(new Lexer());
            var text = StarWarsSchema();
            var schema = parser.Parse(new Source(text));

            var ast = JsonConvert.SerializeObject(schema, Formatting.Indented);
        }
    }
}
