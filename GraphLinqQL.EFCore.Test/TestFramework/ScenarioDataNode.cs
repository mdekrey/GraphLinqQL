using Newtonsoft.Json;
using Xunit.Abstractions;

namespace GraphLinqQL.TestFramework
{

    public class ScenarioDataNode<TGiven, TWhen, TThen> : IXunitSerializable, IScenario
    {
#nullable disable
        public string Name { get; set; }
        public TGiven Given { get; set; }
        public TWhen When { get; set; }
        public TThen Then { get; set; }
#nullable restore

        public void Deserialize(IXunitSerializationInfo info)
        {
            Name = info.GetValue<string>(nameof(Name));
            Given = JsonConvert.DeserializeObject<TGiven>(info.GetValue<string>(nameof(Given)));
            When = JsonConvert.DeserializeObject<TWhen>(info.GetValue<string>(nameof(When)));
            Then = JsonConvert.DeserializeObject<TThen>(info.GetValue<string>(nameof(Then)));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Given), JsonConvert.SerializeObject(Given));
            info.AddValue(nameof(When), JsonConvert.SerializeObject(When));
            info.AddValue(nameof(Then), JsonConvert.SerializeObject(Then));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}