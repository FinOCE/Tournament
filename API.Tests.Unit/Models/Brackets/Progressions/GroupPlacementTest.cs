namespace API.Tests.Unit.Models.Brackets.Progressions;

[TestClass]
public class GroupPlacementTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        Assert.ThrowsException<ArgumentException>(() => new GroupPlacement(0), "0 shouldn't be a valid position");
        Assert.ThrowsException<ArgumentException>(() => new GroupPlacement(-1), "Negative numbers shouldn't be valid positions");
        Assert.IsInstanceOfType(new GroupPlacement(1), typeof(GroupPlacement), "1 should be a valid position");
    }

    [TestMethod]
    public void AddTeamTest()
    {
        SnowflakeService snowflakeService = new();
        Team team1 = new(snowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(snowflakeService.Generate().ToString(), "Team 2", null, false);
        Team team3 = new(snowflakeService.Generate().ToString(), "Team 3", null, false);
        GroupPlacement placement = new(2);

        Assert.IsTrue(placement.AddTeam(team1), "The placement should take team 1");
        Assert.IsTrue(placement.Teams.ContainsKey(team1.Id), "The placement should contain team 1");
        Assert.IsFalse(placement.AddTeam(team1), "The placement should not accept duplicates");
        Assert.IsTrue(placement.AddTeam(team2), "The placement should take team 2");
        Assert.IsTrue(placement.Teams.ContainsKey(team2.Id), "The placement should contain team 2");
        Assert.IsFalse(placement.AddTeam(team3), "The placement should not allow more teams than it's size");
        Assert.AreEqual(2, placement.Teams.Count, "The placement should have two teams total");
    }

    [TestMethod]
    public void RemoveTeamTest()
    {
        SnowflakeService snowflakeService = new();
        Team team1 = new(snowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(snowflakeService.Generate().ToString(), "Team 1", null, false);
        GroupPlacement placement = new(2);

        placement.AddTeam(team1);
        placement.AddTeam(team2);

        Assert.IsTrue(placement.RemoveTeam(team1.Id), "The placement should let team 1 be removed");
        Assert.IsFalse(placement.RemoveTeam(team1.Id), "The placement should not remove a non-existent team");
        Assert.AreEqual(1, placement.Teams.Count, "There should be one remaining team");
    }

    [TestMethod]
    public void ToStringTest()
    {
        Assert.AreEqual("Top 2", new GroupPlacement(2).ToString());
        Assert.AreEqual("Top 4", new GroupPlacement(4).ToString());
        Assert.AreEqual("Top 8", new GroupPlacement(8).ToString());
    }
}
