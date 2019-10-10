namespace GraphLinqQL.TestFramework
{
    class ScenarioData<TGiven, TWhen, TThen> : IScenarioData
    {
#nullable disable
        public string Scenario { get; set; }
        public ScenarioDataNode<TGiven, TWhen, TThen>[] Tests { get; set; }
#nullable restore

        string IScenarioData.Scenario => Scenario;
        IScenario[] IScenarioData.Tests => Tests;
    }
}