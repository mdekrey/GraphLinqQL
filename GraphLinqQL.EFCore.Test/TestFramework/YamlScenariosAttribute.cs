using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Sdk;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphLinqQL.TestFramework
{
    internal class YamlScenariosAttribute : DataAttribute
    {
        private readonly string prefix;

        public YamlScenariosAttribute(string prefix)
        {
            this.prefix = prefix;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var parameters = testMethod.GetParameters();
            var scenarioType = typeof(ScenarioData<,,>)
                .MakeGenericType(parameters.Single().ParameterType.GetGenericArguments());

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();

            var asm = this.GetType().Assembly;
            foreach (var name in asm.GetManifestResourceNames().Where(n => n.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                using var stream = asm.GetManifestResourceStream(name);
                if (stream == null)
                {
                    continue;
                }
                using var reader = new StreamReader(stream);

                var scenario = (IScenarioData)deserializer.Deserialize(reader, scenarioType)!;

                foreach (var test in scenario.Tests)
                {
                    yield return new[] { test };
                }
            }
        }
    }
}
