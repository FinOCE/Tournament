namespace API.Tests.Unit.Models;

[TestClass]
public class GameTest
{
    public SnowflakeService SnowflakeService = null!;
    public Series Series = null!;
    public Game Game = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        Dictionary<string, Team> teams = new();
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        teams.Add(team1.Id, team1);
        teams.Add(team2.Id, team2);

        Series = new(SnowflakeService.Generate().ToString(), teams, 5);

        Game = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(Game.Id, Game);
    }

    [TestMethod]
    public void WinnerTest_Tie()
    {
        Game.Finish();
        Assert.IsNull(Game.Winner, "There should be no winner in a tied game");
    }

    [TestMethod]
    public void WinnerTest_Difference()
    {
        string winnerId = Game.Series.Teams.Keys.First();

        Game.SetScore(winnerId, 1);
        Assert.IsNull(Game.Winner, "There should be no winner on an unfinished game");

        Game.Finish();
        Assert.AreEqual(winnerId, Game.Winner, $"{winnerId} should be the winning team");
    }

    [TestMethod]
    public void ConstructorTest_New()
    {
        Assert.ThrowsException<ArgumentException>(() => new Game("", Series), "The constructor should not accept an invalid snowflake");
        Assert.IsInstanceOfType(new Game(SnowflakeService.Generate().ToString(), Series), typeof(Game), "The constructor should create a valid game");
    }

    [TestMethod]
    public void ConstructorTest_Existing()
    {
        string id = SnowflakeService.Generate().ToString();

        Dictionary<string, int> invalidScore = new();
        Dictionary<string, int> validScore = new();
        validScore.Add(Series.Teams.Keys.First(), 1);
        validScore.Add(Series.Teams.Keys.Last(), 0);

        Assert.ThrowsException<ArgumentException>(() => new Game("", Series, validScore), "The constructor should not accept an invalid snowflake");
        Assert.ThrowsException<ArgumentException>(() => new Game(id, Series, invalidScore), "The constructor should not accept an empty score");

        invalidScore.Add(Series.Teams.First().Key, 1);
        Assert.ThrowsException<ArgumentException>(() => new Game(id, Series, invalidScore), "The constructor should not accept a partial score");

        invalidScore.Add("First", 2);
        invalidScore.Add("Second", 1);
        Assert.ThrowsException<ArgumentException>(() => new Game(id, Series, invalidScore), "The constructor should not accept an invalid score");

        Assert.IsInstanceOfType(new Game(id, Series, validScore), typeof(Game), "The constructor should create a valid game");
    }

    [TestMethod]
    public void GetScoreTest()
    {
        string teamId = Game.Series.Teams.Keys.First();

        Assert.AreEqual(0, Game.GetScore(teamId), "The game should be able to get the team score");
        Assert.ThrowsException<ArgumentException>(() => Game.GetScore(""), "An invalid snowflake should cause an error");

        Game.SetScore(teamId, 1);
        Assert.AreEqual(1, Game.GetScore(teamId), "The game should be able to get the team score");
    }

    [TestMethod]
    public void SetScoreTest()
    {
        string teamId = Game.Series.Teams.Keys.First();

        Assert.IsFalse(Game.SetScore(teamId, -1), "The team score should unsuccessfully set to -1");
        Assert.IsTrue(Game.SetScore(teamId, 0), "The team score should successfully set to 0");
        Assert.IsTrue(Game.SetScore(teamId, 1), "The team score should successfully set to 1");
        Assert.AreEqual(1, Game.GetScore(teamId), "The team score should equal 1");
    }

    [TestMethod]
    public void FinishTest()
    {
        Assert.IsFalse(Game.Finish(), "The game should unsuccessfully finish a tied game");
        Assert.IsFalse(Game.Finished, "The game should not be finished on a tied game");

        string teamId = Game.Series.Teams.Keys.First();
        Game.SetScore(teamId, 1);

        Assert.IsTrue(Game.Finish(), "The game should successfully finish on a score difference");
        Assert.IsTrue(Game.Finished, "A score difference should result in a finished game");
    }
}
