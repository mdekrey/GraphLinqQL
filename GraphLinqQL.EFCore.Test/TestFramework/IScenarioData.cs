namespace GraphLinqQL.TestFramework
{
    interface IScenarioData
    {
        string Scenario { get; }
        IScenario[] Tests { get; }
    }
}