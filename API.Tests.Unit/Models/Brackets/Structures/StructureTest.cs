namespace API.Tests.Unit.Models.Brackets.Structures;

[TestClass]
public class StructureTest
{
    SnowflakeService SnowflakeService = null!;
    Structure Structure = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Series series = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure = new(series);
    }

    [TestMethod]
    public void ChildrenTest()
    {
        // Arrange
        Series series1 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure1 = new(series1);
        Series series2 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure2 = new(series2);
        Series series3 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure3 = new(series3);

        // Act
        Structure.AddChild(structure1);
        Structure.AddChild(structure2);
        structure1.AddChild(structure3);

        // Asset
        Assert.AreEqual(3, Structure.Children, "There should be 3 structures as children");
    }

    [TestMethod]
    public void SetWinnerProgressionTest()
    {
        // Arrange
        ExactPlacement placement = new(1);

        // Act
        Structure.SetWinnerProgression(placement);

        // Assert
        Assert.IsNotNull(Structure.Series.WinnerProgression, "The series winner progression should be set");
    }

    [TestMethod]
    public void SetLoserProgressionTest()
    {
        // Arrange
        ExactPlacement placement = new(2);

        // Act
        Structure.SetLoserProgression(placement);

        // Assert
        Assert.IsNotNull(Structure.Series.LoserProgression, "The series winner progression should be set");
    }

    [TestMethod]
    public void FindStructureWithTeamTest_Single()
    {
        // Arrange
        Team team = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Structure.Series.AddTeam(team);

        // Act
        IStructure? structure = Structure.FindStructureWithTeam(team.Id);

        // Assert
        Assert.IsNotNull(structure, "The structure itself should be found");
        Assert.IsTrue(structure.Series.Teams.ContainsKey(team.Id), "Team 1 should be in the series");
    }

    [TestMethod]
    public void FindStructureWithTeamTest_StartNested()
    {
        // Arrange
        Series series1 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure1 = new(series1);
        Series series2 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure2 = new(series2);
        Series series3 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure3 = new(series3);

        Team team = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        structure3.Series.AddTeam(team);

        Structure.AddChild(structure1);
        Structure.AddChild(structure2);
        structure1.AddChild(structure3);

        // Act
        IStructure? structure = Structure.FindStructureWithTeam(team.Id);

        // Assert
        Assert.IsNotNull(structure, "The structure itself should be found");
        Assert.IsTrue(structure.Series.Teams.ContainsKey(team.Id), "Team 1 should be in the nested series");
    }

    [TestMethod]
    public void FindStructureWithTeamTest_ProgressNested()
    {
        // Arrange
        Series series1 = new(SnowflakeService.Generate().ToString(), null, 1);
        Structure structure1 = new(series1);
        Series series2 = new(SnowflakeService.Generate().ToString(), null, 1);
        Structure structure2 = new(series2);
        Series series3 = new(SnowflakeService.Generate().ToString(), null, 1);
        Structure structure3 = new(series3);

        Structure.AddChild(structure1);
        structure1.SetWinnerProgression(Structure.Series);
        Structure.AddChild(structure2);
        structure2.SetWinnerProgression(Structure.Series);
        structure1.AddChild(structure3);
        structure3.SetWinnerProgression(structure1.Series);

        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        structure3.Series.AddTeam(team1);
        structure3.Series.AddTeam(team2);

        structure3.Series.Start();
        SeriesGame game = new(SnowflakeService.Generate().ToString(), structure3.Series);
        game.SetScore(team1.Id, 1);
        game.Finish();
        structure3.Series.Finish();

        // Act
        IStructure? structure = Structure.FindStructureWithTeam(team1.Id);

        // Assert
        Assert.IsNotNull(structure, "The structure itself should be found");
        Assert.IsTrue(structure.Series.Teams.ContainsKey(team1.Id), "Team 1 should be in the nested series");
        Assert.IsTrue(Structure.Right!.Series.Teams.ContainsKey(team1.Id), "Team 1 should have progressed to semi-finals");
    }

    [TestMethod]
    public void AddChildTest()
    {
        // Arrange
        Series series1 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure1 = new(series1);
        Series series2 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure2 = new(series2);
        Series series3 = new(SnowflakeService.Generate().ToString(), null, 3);
        Structure structure3 = new(series3);

        // Act
        bool firstSuccessful = Structure.AddChild(structure1);
        bool secondSuccessful = Structure.AddChild(structure2);
        bool thirdSuccessful = Structure.AddChild(structure3);

        // Assert
        Assert.IsTrue(firstSuccessful, "First child should be added successfully");
        Assert.AreEqual(structure1.Series.Id, Structure.Right!.Series.Id, "First child should be on the right");
        Assert.IsTrue(secondSuccessful, "Second child should be added successfully");
        Assert.AreEqual(structure2.Series.Id, Structure.Left!.Series.Id, "Second child should be on the left");
        Assert.IsFalse(thirdSuccessful, "Third child should not be added successfully");
    }
}
