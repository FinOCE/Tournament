namespace API.Tests.Unit.Models.Socials;

[TestClass]
public class BracketInviteTest
{
    SnowflakeService SnowflakeService = null!;
    IBracketBuilder Builder = null!;
    ITeam Team = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        Builder = new SingleEliminationBuilder(
            SnowflakeService.Generate().ToString(),
            SnowflakeService);

        Team = new Team(
            SnowflakeService.Generate().ToString(),
            "Team");
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Act
        BracketInvite invalidId() => new("", Builder, Team);
        BracketInvite valid() => new(SnowflakeService.Generate().ToString(), Builder, Team);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not work");
        Assert.IsInstanceOfType(valid(), typeof(BracketInvite), "A valid constructor should work");
    }

    [TestMethod]
    public void AcceptTest()
    {
        // Arrange
        BracketInvite invite = new(SnowflakeService.Generate().ToString(), Builder, Team);

        // Act
        bool acceptedBefore = invite.Accepted;
        bool declinedBefore = invite.Declined;

        bool firstCall = invite.Accept();
        bool secondCall = invite.Accept();

        bool acceptAfter = invite.Accepted;
        bool declinedAfter = invite.Declined;

        // Assert
        Assert.IsFalse(acceptedBefore, "The invite should start off not accepted");
        Assert.IsFalse(declinedBefore, "The invite should start off not declined");
        Assert.IsTrue(firstCall, "The invite should be successfully accepted");
        Assert.IsFalse(secondCall, "A duplicate call should not be successful");
        Assert.IsTrue(acceptAfter, "The invite should be marked as accepted");
        Assert.IsFalse(declinedAfter, "The invite should not be declined");
    }

    [TestMethod]
    public void DeclineTest()
    {
        // Arrange
        BracketInvite invite = new(SnowflakeService.Generate().ToString(), Builder, Team);

        // Act
        bool acceptedBefore = invite.Accepted;
        bool declinedBefore = invite.Declined;

        bool firstCall = invite.Decline();
        bool secondCall = invite.Decline();

        bool acceptAfter = invite.Accepted;
        bool declinedAfter = invite.Declined;

        // Assert
        Assert.IsFalse(acceptedBefore, "The invite should start off not accepted");
        Assert.IsFalse(declinedBefore, "The invite should start off not declined");
        Assert.IsTrue(firstCall, "The invite should be successfully declined");
        Assert.IsFalse(secondCall, "A duplicate call should not be successful");
        Assert.IsFalse(acceptAfter, "The invite should not be marked as accepted");
        Assert.IsTrue(declinedAfter, "The invite should be marked as declined");
    }
}
