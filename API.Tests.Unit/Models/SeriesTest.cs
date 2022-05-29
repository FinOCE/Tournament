namespace API.Tests.Unit.Models;

[TestClass]
public class SeriesTest
{
    public SnowflakeService SnowflakeService = null!;
    public Dictionary<string, Team> Teams = null!;
    public Series Series = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        Teams = new();
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Teams.Add(team1.Id, team1);
        Teams.Add(team2.Id, team2);

        Series = new(SnowflakeService.Generate().ToString(), Teams, 3);
    }

    [TestMethod]
    public void ForfeitedTest()
    {
        Assert.IsFalse(Series.Forfeited, "The series should not be marked as forfeited");

        Series.Forfeit(Series.Teams.Keys.First());
        Assert.IsTrue(Series.Forfeited, "The game should be marked as forfeited");
    }

    [TestMethod]
    public void WinnerTest_Win()
    {
        Assert.IsNull(Series.Winner, "There should not be a winner yet");

        string winnerId = Series.Teams.Keys.First();

        Game game1 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game1.Id, game1);
        game1.SetScore(winnerId, 1);
        game1.Finish();

        Assert.IsNull(Series.Winner, "1 win in a best of 3 should not allow a win");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game2.Id, game2);
        game2.SetScore(winnerId, 1);
        game2.Finish();

        Series.Finish();

        Assert.AreEqual(winnerId, Series.Winner, "A winner should be shown after 2/3 wins");
    }

    [TestMethod]
    public void WinnerTest_Forfeit()
    {
        Assert.IsNull(Series.Winner, "There should not be a winner yet");

        Series.Forfeit(Series.Teams.Keys.First());
        Assert.AreEqual(Series.Teams.Keys.Last(), Series.Winner, "The team that didn't forfeit should be the winner");
    }

    [TestMethod]
    public void ScoreTest()
    {
        string team1Id = Series.Teams.Keys.First();
        string team2Id = Series.Teams.Keys.Last();

        Assert.AreEqual(0, Series.Score[team1Id], "Team 1 score should be 0");
        Assert.AreEqual(0, Series.Score[team2Id], "Team 2 score should be 0");

        Game game1 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game1.Id, game1);
        game1.SetScore(team1Id, 1);
        game1.Finish();

        Assert.AreEqual(1, Series.Score[team1Id], "Team 1 score should be 1");
        Assert.AreEqual(0, Series.Score[team2Id], "Team 2 score should be 0");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game2.Id, game2);
        game2.SetScore(team2Id, 1);
        game2.Finish();

        Assert.AreEqual(1, Series.Score[team1Id], "Team 1 score should be 1");
        Assert.AreEqual(1, Series.Score[team2Id], "Team 2 score should be 1");

        Game game3 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game3.Id, game3);
        game3.SetScore(team1Id, 1);
        game3.Finish();

        Assert.AreEqual(2, Series.Score[team1Id], "Team 1 score should be 2");
        Assert.AreEqual(1, Series.Score[team2Id], "Team 2 score should be 1");
    }

    [TestMethod]
    public void ConstructorTest_New()
    {
        Assert.ThrowsException<ArgumentException>(() => new Series("", Teams, 3), "An invalid snowflake should throw an error");
        Assert.ThrowsException<ArgumentException>(() => new Series(SnowflakeService.Generate().ToString(), Teams, 0), "An invalid best of count should throw an error");
        Assert.IsInstanceOfType(new Series(SnowflakeService.Generate().ToString(), Teams, 3), typeof(Series), "A series should be instantiated");
    }

    [TestMethod]
    public void ConstructorTest_Existing()
    {
        // Create the games history to be used when testing
        Dictionary<string, Game> games = new();

        Game game = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game.Id, game);
        game.SetScore(Series.Teams.Keys.First(), 1);
        game.Finish();

        // Test various forms of the constructor error handling
        Assert.ThrowsException<ArgumentException>(() => new Series("", Teams, 3, games), "An invalid snowflake should throw an error");
        Assert.ThrowsException<ArgumentException>(() => new Series(SnowflakeService.Generate().ToString(), Teams, 0, games), "An invalid best of count should throw an error");
        Assert.IsInstanceOfType(new Series(SnowflakeService.Generate().ToString(), Teams, 3, games), typeof(Series), "A series should be instantiated");

        games.Add(game.Id, game);
        Assert.IsInstanceOfType(new Series(SnowflakeService.Generate().ToString(), Teams, 3, games), typeof(Series), "A series should be instantiated");

        // Test a game from a different series
        Dictionary<string, Team> teams = new();
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        teams.Add(team1.Id, team1);
        teams.Add(team2.Id, team2);

        Series invalidSeries = new(SnowflakeService.Generate().ToString(), Teams, 3);

        Game invalidGame = new(SnowflakeService.Generate().ToString(), invalidSeries);
        games.Add(invalidGame.Id, invalidGame);
        Assert.ThrowsException<ArgumentException>(() => new Series("", Teams, 3, games), "A game from a different series shouldn't work");
    }

    [TestMethod]
    public void FinishTest()
    {
        string team1Id = Series.Teams.Keys.First();
        string team2Id = Series.Teams.Keys.Last();

        Game game1 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game1.Id, game1);
        game1.SetScore(team1Id, 1);
        game1.Finish();

        Assert.IsFalse(Series.Finish(), "The series should unsuccessfully finish an incomplete series");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game2.Id, game2);
        game2.SetScore(team2Id, 1);
        game2.Finish();

        Assert.IsFalse(Series.Finish(), "The series should unsuccessfully finish an incomplete series");

        Game game3 = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game3.Id, game3);
        game3.SetScore(team1Id, 1);
        game3.Finish();

        Assert.IsTrue(Series.Finish(), "The series should successfully finish with a team having 2/3 wins");
        Assert.IsFalse(Series.Finish(), "A game should not be able to be finished multiple times");
    }

    [TestMethod]
    public void ForfeitTest()
    {
        string forfeitingTeamId = Series.Teams.Keys.First();

        Game game = new(SnowflakeService.Generate().ToString(), Series);
        Series.Games.Add(game.Id, game);
        game.SetScore(forfeitingTeamId, 1);
        game.Finish();

        Assert.IsFalse(Series.Forfeit("Invalid ID"), "An invalid ID should not be able to forfeit");
        Assert.IsTrue(Series.Forfeit(forfeitingTeamId), "The team should be able to forfeit");
        Assert.IsTrue(Series.Forfeited && Series.Finished, "The game should be marked as forfeited and completed");
        Assert.IsFalse(Series.Forfeit(forfeitingTeamId), "A game should not be able to be forfeited multiple times");
    }
}
