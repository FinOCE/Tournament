namespace API.Tests.Unit.Models;

[TestClass]
public class SeriesTest
{
    public SnowflakeService SnowflakeService = null!;
    public Dictionary<string, Team> Teams = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        Teams = new();
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Teams.Add(team1.Id, team1);
        Teams.Add(team2.Id, team2);
    }

    [TestMethod]
    public void ForfeitedTest()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void WinnerTest()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void ScoreTest()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void ConstructorTest_New()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void ConstructorTest_Existing()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void FinishTest()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void ForfeitTest()
    {
        throw new NotImplementedException();
    }
}
