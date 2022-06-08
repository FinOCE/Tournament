namespace API.Tests.Unit.Models.Brackets.Builders;

[TestClass]
public class SingleElminationBuilderTest
{
    SnowflakeService SnowflakeService = null!;
    SingleEliminationBuilder Builder = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Builder = new(SnowflakeService.Generate().ToString(), SnowflakeService);
    }

    /// <summary>
    /// Add the given number of teams to the builder
    /// </summary>
    private void AddTeams(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Team team = new(SnowflakeService.Generate().ToString(), $"Team {i + 1}", null, false);
            Builder.AddTeam(team, count - i);
        }
    }

    private void AddSoloTeams(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SoloTeam team = new(new User(SnowflakeService.Generate().ToString(), $"User {i + 1}", 1234));
            Builder.AddTeam(team, count - i);
        }
    }

    [TestMethod]
    public void GenerateTest_00Teams()
    {
        // Arrange
        AddTeams(0);

        // Act
        IStructure generate() => Builder.Generate();

        // Assert
        Assert.ThrowsException<InvalidOperationException>(generate, "A bracket cannot be built with no teams");
    }

    [TestMethod]
    public void GenerateTest_01Teams()
    {
        // Arrange
        AddSoloTeams(1);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.AreEqual(1, root.Series.Teams.Count, "There should only be one team in the bracket");
        Assert.IsInstanceOfType(root.Series.WinnerProgression, typeof(ExactPlacement), "The winner should proceed to an ExactPlacement");
        Assert.AreEqual(1, ((ExactPlacement)root.Series.WinnerProgression!).Position, "The winner progression position should be 1");
        Assert.AreEqual(0, root.Children, "There should be no preceeding structures");
    }

    [TestMethod]
    public void GenerateTest_02Teams()
    {
        // Arrange
        AddTeams(2);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.AreEqual(2, root.Series.Teams.Count, "There should be two teams in the bracket");
        Assert.IsInstanceOfType(root.Series.WinnerProgression, typeof(ExactPlacement), "The winner should proceed to an ExactPlacement");
        Assert.AreEqual(1, ((ExactPlacement)root.Series.WinnerProgression!).Position, "The winner progression position should be 1");
        Assert.IsInstanceOfType(root.Series.WinnerProgression, typeof(ExactPlacement), "The loser should proceed to an ExactPlacement");
        Assert.AreEqual(2, ((ExactPlacement)root.Series.LoserProgression!).Position, "The loser progression position should be 2");
        Assert.AreEqual(0, root.Children, "There should be no preceeding structures");
    }

    [TestMethod]
    public void GenerateTest_03Teams()
    {
        // Arrange
        AddSoloTeams(3);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.IsNull(root.Left, "The left child should not exist");
        Assert.IsNotNull(root.Right, "The right child should exist");
        Assert.IsTrue(root.Series.Teams.Values.Any(t => t.Name == "User 1"), "Team 1 should be in the final");
        Assert.AreEqual(1, root.Series.Teams.Count, "There should only be one team in the finals");
        Assert.IsTrue(root.Right.Series.Teams.Values.Any(t => t.Name == "User 2"), "Team 2 should be in the semi-final");
        Assert.IsTrue(root.Right.Series.Teams.Values.Any(t => t.Name == "User 3"), "Team 3 should be in the semi-final");
        Assert.AreEqual(2, root.Right.Series.Teams.Count, "There should be two teams in the semi-final");
        Assert.AreEqual(1, root.Children, "There should only be the final and single semi-final series");
    }

    [TestMethod]
    public void GenerateTest_04Teams()
    {
        // Arrange
        AddTeams(4);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.IsNotNull(root.Left, "The left child should exist");
        Assert.IsNotNull(root.Right, "The right child should exist");
        Assert.AreEqual(0, root.Series.Teams.Count, "There should be no teams in the final");
        Assert.IsTrue(root.Left.Series.Teams.Values.Any(t => t.Name == "Team 1"), "Team 1 should be in the left semi-final");
        Assert.IsTrue(root.Left.Series.Teams.Values.Any(t => t.Name == "Team 4"), "Team 4 should be in the left semi-final");
        Assert.AreEqual(2, root.Left.Series.Teams.Count, "There should be two teams in the left semi-final");
        Assert.IsTrue(root.Right.Series.Teams.Values.Any(t => t.Name == "Team 2"), "Team 2 should be in the right semi-final");
        Assert.IsTrue(root.Right.Series.Teams.Values.Any(t => t.Name == "Team 3"), "Team 3 should be in the right semi-final");
        Assert.AreEqual(2, root.Right.Series.Teams.Count, "There should be two teams in the right semi-final");
        Assert.AreEqual(2, root.Children, "There should only be the final and two semi-final series");
    }

    [TestMethod]
    public void GenerateTest_09Teams()
    {
        // Arrange
        AddSoloTeams(9);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.IsTrue(root.Left!.Left!.Series.Teams.Values.Any(t => t.Name == "User 1"), "Team 1 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "User 2"), "Team 2 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "User 3"), "Team 3 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "User 4"), "Team 4 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "User 5"), "Team 5 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "User 6"), "Team 6 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "User 7"), "Team 7 should be in this series");
        Assert.IsTrue(root.Left!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "User 8"), "Team 8 should be in this series");
        Assert.IsTrue(root.Left!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "User 9"), "Team 9 should be in this series");
    }

    [TestMethod]
    public void GenerateTest_16Teams()
    {
        // Arrage
        AddTeams(16);

        // Act
        IStructure root = Builder.Generate();

        // Assert
        Assert.IsTrue(root.Left!.Left!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 1"), "Team 1 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 2"), "Team 2 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 3"), "Team 3 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 4"), "Team 4 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 5"), "Team 5 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 6"), "Team 6 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 7"), "Team 7 should be in this series");
        Assert.IsTrue(root.Left!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 8"), "Team 8 should be in this series");
        Assert.IsTrue(root.Left!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 9"), "Team 9 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 10"), "Team 10 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 11"), "Team 11 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Right!.Series.Teams.Values.Any(t => t.Name == "Team 12"), "Team 12 should be in this series");
        Assert.IsTrue(root.Left!.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 13"), "Team 13 should be in this series");
        Assert.IsTrue(root.Right!.Right!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 14"), "Team 14 should be in this series");
        Assert.IsTrue(root.Right!.Left!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 15"), "Team 15 should be in this series");
        Assert.IsTrue(root.Left!.Left!.Left!.Series.Teams.Values.Any(t => t.Name == "Team 16"), "Team 16 should be in this series");
    }

    [TestMethod]
    public void PlaythroughTest()
    {
        // Arrange
        AddSoloTeams(6);
        Builder.SetBestOf(1);

        // Act
        IStructure root = Builder.Generate();

        // Game 1: Team 3 vs 6 - Winner: 3 (Quarter Finals)
        IStructure structure1 = root.FindStructureWithTeam(Builder.Teams.Values.First(t => t.Name == "User 3").Id)!;
        structure1.Series.Start();
        SeriesGame game1 = new(SnowflakeService.Generate().ToString(), structure1.Series);
        game1.SetScore(structure1.Series.Teams.Values.First(t => t.Name == "User 3").Id, 1);
        game1.Finish();
        structure1.Series.Finish();

        // Game 2: Team 4 vs 5 - Winner: 5 (Quarter Finals)
        IStructure structure2 = root.FindStructureWithTeam(Builder.Teams.Values.First(t => t.Name == "User 4").Id)!;
        structure2.Series.Start();
        SeriesGame game2 = new(SnowflakeService.Generate().ToString(), structure2.Series);
        game2.SetScore(structure2.Series.Teams.Values.First(t => t.Name == "User 5").Id, 1);
        game2.Finish();
        structure2.Series.Finish();

        // Game 3: Team 2 vs 3 - Winner: 2 (Semi Finals)
        IStructure structure3 = root.FindStructureWithTeam(Builder.Teams.Values.First(t => t.Name == "User 2").Id)!;
        structure3.Series.Start();
        SeriesGame game3 = new(SnowflakeService.Generate().ToString(), structure3.Series);
        game3.SetScore(structure3.Series.Teams.Values.First(t => t.Name == "User 2").Id, 1);
        game3.Finish();
        structure3.Series.Finish();

        // Game 4: Team 1 vs 5 - Winner: 1 (Semi Finals)
        IStructure structure4 = root.FindStructureWithTeam(Builder.Teams.Values.First(t => t.Name == "User 1").Id)!;
        structure4.Series.Start();
        SeriesGame game4 = new(SnowflakeService.Generate().ToString(), structure4.Series);
        game4.SetScore(structure4.Series.Teams.Values.First(t => t.Name == "User 1").Id, 1);
        game4.Finish();
        structure4.Series.Finish();

        // Game 5: Team 1 vs 2 - Winner: 1 (Finals)
        IStructure structure5 = root.FindStructureWithTeam(Builder.Teams.Values.First(t => t.Name == "User 1").Id)!;
        structure5.Series.Start();
        SeriesGame game5 = new(SnowflakeService.Generate().ToString(), structure5.Series);
        game5.SetScore(structure5.Series.Teams.Values.First(t => t.Name == "User 1").Id, 1);
        game5.Finish();
        structure5.Series.Finish();

        // Assert
        root.Series.WinnerProgression!.Teams.ContainsKey(Builder.Teams.Values.First(t => t.Name == "User 1").Id);
    }
}
