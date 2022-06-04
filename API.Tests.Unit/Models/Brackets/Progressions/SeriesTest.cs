namespace API.Tests.Unit.Models.Brackets.Progressions;

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
    public void StartedTest()
    {
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Series series = new(SnowflakeService.Generate().ToString(), null, 3);

        Assert.IsFalse(series.Started, "The series should not start with no teams");

        series.AddTeam(team1);
        series.AddTeam(team2);
        Assert.IsFalse(series.Started, "The series should not start without being called");

        series.Start();
        Assert.IsTrue(series.Started, "The series should start with two teams");
    }

    [TestMethod]
    public void FinishedTest()
    {
        string team1Id = Series.Teams.Keys.First();
        string team2Id = Series.Teams.Keys.Last();

        Game game1 = new(SnowflakeService.Generate().ToString(), Series);
        game1.SetScore(team1Id, 1);
        game1.Finish();
        Series.Finish();

        Assert.IsFalse(Series.Finished, "The series should unsuccessfully finish an incomplete series");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        game2.SetScore(team2Id, 1);
        game2.Finish();
        Series.Finish();

        Assert.IsFalse(Series.Finished, "The series should unsuccessfully finish an incomplete series");

        Game game3 = new(SnowflakeService.Generate().ToString(), Series);
        game3.SetScore(team1Id, 1);
        game3.Finish();
        Series.Finish();

        Assert.IsTrue(Series.Finished, "The series should successfully finish with a team having 2/3 wins");
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
        game1.SetScore(winnerId, 1);
        game1.Finish();

        Assert.IsNull(Series.Winner, "1 win in a best of 3 should not allow a win");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
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
        game1.SetScore(team1Id, 1);
        game1.Finish();

        Assert.AreEqual(1, Series.Score[team1Id], "Team 1 score should be 1");
        Assert.AreEqual(0, Series.Score[team2Id], "Team 2 score should be 0");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        game2.SetScore(team2Id, 1);
        game2.Finish();

        Assert.AreEqual(1, Series.Score[team1Id], "Team 1 score should be 1");
        Assert.AreEqual(1, Series.Score[team2Id], "Team 2 score should be 1");

        Game game3 = new(SnowflakeService.Generate().ToString(), Series);
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
    public void AddTeamTest()
    {
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Team team3 = new(SnowflakeService.Generate().ToString(), "Team 3", null, false);
        Series series = new(SnowflakeService.Generate().ToString(), null, 3);

        Assert.AreEqual(0, series.Teams.Count, "The series should be created with no teams");

        Assert.IsTrue(series.AddTeam(team1), "Team 1 should successfully be added");
        Assert.AreEqual(1, series.Teams.Count, "The series should have 1 team");

        Assert.IsFalse(series.AddTeam(team1), "Team 1 should not be able to be added twice");
        Assert.AreEqual(1, series.Teams.Count, "The series should have 1 team");

        Assert.IsTrue(series.AddTeam(team2), "Team 2 should successfully be added");
        Assert.AreEqual(2, series.Teams.Count, "The series should have 2 teams");

        Assert.IsFalse(series.AddTeam(team3), "Team 3 should not fit in the series");
        Assert.AreEqual(2, series.Teams.Count, "The series should have 2 teams");
    }

    [TestMethod]
    public void RemoveTeamTest()
    {
        Team team1 = Teams.Values.First();
        Team team2 = Teams.Values.Last();

        Assert.AreEqual(2, Series.Teams.Count, "There should be 2 teams in the series");

        Assert.IsTrue(Series.RemoveTeam(team1.Id), "Team 1 should successfully be removed");
        Assert.AreEqual(1, Series.Teams.Count, "There should only be 1 remaining team");
        Assert.IsFalse(Series.RemoveTeam(team1.Id), "A team that isn't in the series should not be removed");

        Assert.IsTrue(Series.RemoveTeam(team2.Id), "Team 2 should successfully be removed");
        Assert.AreEqual(0, Series.Teams.Count, "There should be no remaining teams");
    }

    [TestMethod]
    [Ignore]
    public void SetBestOfTest()
    { 
    }

    [TestMethod]
    public void StartTest()
    {
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Series series = new(SnowflakeService.Generate().ToString(), null, 3);

        Assert.IsFalse(series.Start(), "The series should not start with no teams");
        Assert.IsNull(series.StartedTimestamp, "The started timestamp should not exist");

        series.AddTeam(team1);
        series.AddTeam(team2);
        Assert.IsTrue(series.Start(), "The series should start with two teams");
        Assert.IsNotNull(series.StartedTimestamp, "The started timestamp should exist");
    }

    [TestMethod]
    public void FinishTest()
    {
        Series winnerSeries = new(SnowflakeService.Generate().ToString(), null, 3);
        Series.SetWinnerProgression(winnerSeries);
        Series loserSeries = new(SnowflakeService.Generate().ToString(), null, 3);
        Series.SetLoserProgression(loserSeries);

        string team1Id = Series.Teams.Keys.First();
        string team2Id = Series.Teams.Keys.Last();

        Game game1 = new(SnowflakeService.Generate().ToString(), Series);
        game1.SetScore(team1Id, 1);
        game1.Finish();

        Assert.IsFalse(Series.Finish(), "The series should unsuccessfully finish an incomplete series");

        Game game2 = new(SnowflakeService.Generate().ToString(), Series);
        game2.SetScore(team2Id, 1);
        game2.Finish();

        Assert.IsFalse(Series.Finish(), "The series should unsuccessfully finish an incomplete series");

        Game game3 = new(SnowflakeService.Generate().ToString(), Series);
        game3.SetScore(team1Id, 1);
        game3.Finish();

        Assert.IsTrue(Series.Finish(), "The series should successfully finish with a team having 2/3 wins");
        Assert.IsFalse(Series.Finish(), "A game should not be able to be finished multiple times");

        Assert.IsTrue(winnerSeries.Teams.ContainsKey(team1Id), "The winner should have progressed to winner series");
        Assert.IsTrue(loserSeries.Teams.ContainsKey(team2Id), "The loser should have progressed to loser series");
    }

    [TestMethod]
    public void ForfeitTest()
    {
        string forfeitingTeamId = Series.Teams.Keys.First();

        Game game = new(SnowflakeService.Generate().ToString(), Series);
        game.SetScore(forfeitingTeamId, 1);
        game.Finish();

        Assert.IsFalse(Series.Forfeit("Invalid ID"), "An invalid ID should not be able to forfeit");
        Assert.IsTrue(Series.Forfeit(forfeitingTeamId), "The team should be able to forfeit");
        Assert.IsTrue(Series.Forfeited && Series.Finished, "The game should be marked as forfeited and completed");
        Assert.IsFalse(Series.Forfeit(forfeitingTeamId), "A game should not be able to be forfeited multiple times");
    }

    [TestMethod]
    public void SetWinnerProgressionTest()
    {
        Assert.IsNull(Series.WinnerProgression, "There should be no winner progression by default");

        Series winnerSeries = new(SnowflakeService.Generate().ToString(), null, 3);
        Series.SetWinnerProgression(winnerSeries);

        Assert.AreEqual(winnerSeries.Id, ((Series)Series.WinnerProgression!).Id, "The series should be applied as the winner series");
    }

    [TestMethod]
    public void SetLoserProgressionTest()
    {
        Assert.IsNull(Series.LoserProgression, "There should be no loser progression by default");

        Series loserSeries = new(SnowflakeService.Generate().ToString(), null, 3);
        Series.SetLoserProgression(loserSeries);

        Assert.AreEqual(loserSeries.Id, ((Series)Series.LoserProgression!).Id, "The series should be applied as the loser series");
    }
}
