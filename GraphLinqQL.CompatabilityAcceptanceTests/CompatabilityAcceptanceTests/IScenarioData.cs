namespace GraphLinqQL.CompatabilityAcceptanceTests
{
    interface IScenarioData
    {
        string Scenario { get; }
        object[] Tests { get; }
    }
}