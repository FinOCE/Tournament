namespace API.Tests.Unit.Models.Brackets.Builders;

[TestClass]
public class SingleElminationBuilderTest
{
    [TestMethod]
    [Ignore]
    public void GenerateTest()
    {
        SnowflakeService snowflakeService = new();
        IBracketBuilder bracket = new SingleEliminationBuilder(snowflakeService);
        int numberOfTeams = 9;

        for (int i = 0; i < numberOfTeams; i++)
        {
            Team team = new(snowflakeService.Generate().ToString(), $"Team {i + 1}", null, false);
            bracket.AddTeam(team, numberOfTeams - i);
        }

        bracket.Generate();
    }
}
