namespace API.Tests.Unit.Models.Teams;

[TestClass]
public class SoloTeamTest
{
    SnowflakeService SnowflakeService = null!;
    User User = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        User = new(SnowflakeService.Generate().ToString(), "User", 1234);
    }

    [TestMethod]
    public void IdTest()
    {
        // Arrange
        ITeam team = new SoloTeam(User);

        // Act
        string id = team.Id;

        // Assert
        Assert.AreEqual(User.Id, id, "The team ID should be the user's ID");
    }

    [TestMethod]
    public void NameTest()
    {
        // Arrange
        ITeam team = new SoloTeam(User);

        // Act
        string name = team.Name;

        // Assert
        Assert.AreEqual(User.Username, name, "The team name should be the user's name");
    }

    [TestMethod]
    public void IconTest()
    {
        // Arrange
        ITeam team = new SoloTeam(User);

        // Act
        string icon = team.Icon;

        // Assert
        Assert.AreEqual(User.Icon, icon, "The team icon should be the user's icon");
    }

    [TestMethod]
    public void VerifiedTest()
    {
        // Arrange
        ITeam team = new SoloTeam(User);

        // Act
        bool verified = team.Verified;

        // Assert
        Assert.IsFalse(User.Verified, "The user should not be verified by default");
        Assert.IsFalse(verified, "The team should not be verified since the user isn't");
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Act
        ITeam team = new SoloTeam(User);

        // Assert
        Assert.IsInstanceOfType(team, typeof(SoloTeam), "A SoloTeam should be successfully made");
    }
}
