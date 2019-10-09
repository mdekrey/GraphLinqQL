namespace GraphLinqQL.TestFramework
{
    interface IScenarioData
    {
        string Scenario { get; }
        object[] Tests { get; }
    }
}