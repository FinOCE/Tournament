namespace API.Tests.Unit.Models.Brackets.Builders;

[TestClass]
public class BracketBuilderTest
{
    /// <summary>
    /// A non-abstract superclass of BracketBuilder to use for testing
    /// </summary>
    public class NonAbstractBuilder : BracketBuilder
    {
        public NonAbstractBuilder(string id, SnowflakeService snowflakeService) : base(id, snowflakeService) { }
        
        /// <summary>
        /// A non-abstract override to fulfil the requirements for the superclass (unused)
        /// </summary>
        public override IStructure Generate()
        {
            return new Structure(new Series(SnowflakeService.Generate().ToString(), null, 1));
        }
    }

    SnowflakeService SnowflakeService = null!;
    NonAbstractBuilder Builder = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Builder = new NonAbstractBuilder(SnowflakeService.Generate().ToString(), SnowflakeService);
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Act
        NonAbstractBuilder invalidId() => new("", SnowflakeService);
        NonAbstractBuilder valid() => new(SnowflakeService.Generate().ToString(), SnowflakeService);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should thow an exception");
        Assert.IsInstanceOfType(valid(), typeof(NonAbstractBuilder), "A valid constructor should work");
    }

    [TestMethod]
    public void AddTeamTest_NoSeed()
    {
        // Arrange
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);

        // Act
        Builder.AddTeam(team1);
        Builder.AddTeam(team2);

        // Assert
        Assert.IsTrue(Builder.Teams.ContainsKey(team1.Id), "Team 1 should be in the builder's teams");
        Assert.IsTrue(Builder.Seeds.ContainsKey(team1.Id), "Team 2 should be in the builder's teams");
        Assert.IsTrue(Builder.Teams.ContainsKey(team2.Id), "Team 1 should have their ID with a seed");
        Assert.IsTrue(Builder.Seeds.ContainsKey(team2.Id), "Team 2 should have their ID with a seed");
        Assert.IsFalse(Builder.AddTeam(team1), "The builder should not accept a team that is already included");
    }

    [TestMethod]
    public void AddTeamTest_WithSeed()
    {
        // Arrange
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);

        // Act
        Builder.AddTeam(team1, 1);
        Builder.AddTeam(team2, 10);
        bool duplicateAddition = Builder.AddTeam(team1, 5);

        // Assert
        Assert.IsTrue(Builder.Teams.ContainsKey(team1.Id), "Team 1 should be in the builder's teams");
        Assert.AreEqual(1, Builder.Seeds[team1.Id], "Team 1 should have a seed value of 1");
        Assert.IsTrue(Builder.Teams.ContainsKey(team2.Id), "Team 2 should be in the builder's teams");
        Assert.AreEqual(10, Builder.Seeds[team2.Id], "Team 2 should have a seed value of 10");
        Assert.IsFalse(duplicateAddition, "The builder should not accept a team that is already included");
    }

    [TestMethod]
    public void RemoveTeamTest()
    {
        // Arrange
        Team team1 = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(SnowflakeService.Generate().ToString(), "Team 2", null, false);
        Builder.AddTeam(team1);
        Builder.AddTeam(team2);

        // Act
        Builder.RemoveTeam(team1.Id);
        bool duplicateRemoval = Builder.RemoveTeam(team1.Id);

        // Assert
        Assert.IsFalse(Builder.Teams.ContainsKey(team1.Id), "Team 1 should no longer be in the builder");
        Assert.IsTrue(Builder.Teams.ContainsKey(team2.Id), "Team 2 should still be in the builder");
        Assert.IsFalse(duplicateRemoval, "The builder should not be able to remove a team twice");
    }

    [TestMethod]
    public void SetSeedTest()
    {
        // Arrange
        Team team = new(SnowflakeService.Generate().ToString(), "Team 1", null, false);
        Builder.AddTeam(team, 5);

        // Act
        Builder.SetSeed(team.Id, 10);

        // Assert
        Assert.AreEqual(10, Builder.Seeds[team.Id], "The seed should be updated to 10");
    }

    [TestMethod]
    public void SetBestOfTest()
    {
        // Act
        bool setTo0 = Builder.SetBestOf(0);
        int valueAfterSetTo0 = Builder.BestOf;
        bool setTo5 = Builder.SetBestOf(5);
        int valueAfterSetTo5 = Builder.BestOf;

        // Assert
        Assert.IsFalse(setTo0, "The seed should not be able to be set to 0 or below");
        Assert.AreEqual(1, valueAfterSetTo0, "The seed should remain unchanged on invalid call");
        Assert.IsTrue(setTo5, "The seed should be successfully set to 5");
        Assert.AreEqual(5, valueAfterSetTo5, "The seed should be updated to 5");
    }

    [TestMethod]
    public void GetOrderedTeamsTest()
    {
        // Arrange
        Random random = new();

        for (int i = 0; i < 10; i++)
        {
            Team team = new(SnowflakeService.Generate().ToString(), $"Team {i + 1}", null, false);
            Builder.AddTeam(team, random.Next(0, 100));
        }

        // Act
        ITeam[] teams = Builder.GetOrderedTeams();
        bool isOutOfOrder = false;
        int previousSeed = int.MinValue;

        foreach (ITeam team in teams)
            if (Builder.Seeds[team.Id] < previousSeed)
            {
                isOutOfOrder = true;
                break;
            }

        // Assert
        Assert.IsFalse(isOutOfOrder, "The teams should not be out of order");
        Assert.AreEqual(10, teams.Length, "There should still be 10 teams in the list");
    }
}
