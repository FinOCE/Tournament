namespace API.Tests.Unit.Models.Brackets.Progressions;

[TestClass]
public class ExactPlacementTest
{
    [TestMethod]
    public void ConstructorTest()
    {
        Assert.ThrowsException<ArgumentException>(() => new ExactPlacement(0), "0 shouldn't be a valid position");
        Assert.ThrowsException<ArgumentException>(() => new ExactPlacement(-1), "Negative numbers shouldn't be valid positions");
        Assert.IsInstanceOfType(new ExactPlacement(1), typeof(ExactPlacement), "1 should be a valid position");
    }

    [TestMethod]
    public void AddTeamTest()
    {
        SnowflakeService snowflakeService = new();
        Team team1 = new(snowflakeService.Generate().ToString(), "Team 1", null, false);
        Team team2 = new(snowflakeService.Generate().ToString(), "Team 2", null, false);
        ExactPlacement placement = new(2);

        Assert.IsTrue(placement.AddTeam(team1), "The placement should take team 1");
        Assert.IsTrue(placement.Teams.ContainsKey(team1.Id), "The placement should contain team 1");
        Assert.IsFalse(placement.AddTeam(team1), "The placement should not accept duplicates");
        Assert.IsFalse(placement.AddTeam(team2), "The placement should not take a second team");
        Assert.IsFalse(placement.Teams.ContainsKey(team2.Id), "The placement should contain team 2");
        Assert.AreEqual(1, placement.Teams.Count, "The placement should have only one team");
    }

    [TestMethod]
    public void RemoveTeamTest()
    {
        SnowflakeService snowflakeService = new();
        Team team = new(snowflakeService.Generate().ToString(), "Team 1", null, false);
        ExactPlacement placement = new(2);

        placement.AddTeam(team);

        Assert.IsTrue(placement.RemoveTeam(team.Id), "The placement should let team 1 be removed");
        Assert.IsFalse(placement.RemoveTeam(team.Id), "The placement should not remove a non-existent team");
        Assert.AreEqual(0, placement.Teams.Count, "There should be no team in the position");
    }

    [TestMethod]
    public void ToStringTest()
    {
        Assert.AreEqual("1st", new ExactPlacement(1).ToString());
        Assert.AreEqual("2nd", new ExactPlacement(2).ToString());
        Assert.AreEqual("3rd", new ExactPlacement(3).ToString());
        Assert.AreEqual("4th", new ExactPlacement(4).ToString());
        Assert.AreEqual("10th", new ExactPlacement(10).ToString());
        Assert.AreEqual("11th", new ExactPlacement(11).ToString());
        Assert.AreEqual("12th", new ExactPlacement(12).ToString());
        Assert.AreEqual("13th", new ExactPlacement(13).ToString());
        Assert.AreEqual("14th", new ExactPlacement(14).ToString());
        Assert.AreEqual("21st", new ExactPlacement(21).ToString());
        Assert.AreEqual("22nd", new ExactPlacement(22).ToString());
        Assert.AreEqual("23rd", new ExactPlacement(23).ToString());
        Assert.AreEqual("24th", new ExactPlacement(24).ToString());
        Assert.AreEqual("101st", new ExactPlacement(101).ToString());
        Assert.AreEqual("111th", new ExactPlacement(111).ToString());
        Assert.AreEqual("121st", new ExactPlacement(121).ToString());
    }
}
