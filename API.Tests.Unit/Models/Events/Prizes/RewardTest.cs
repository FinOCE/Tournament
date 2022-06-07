namespace API.Tests.Unit.Models.Events.Prizes;

[TestClass]
public class PrizesTest
{
    SnowflakeService SnowflakeService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Arrange
        string id = SnowflakeService.Generate().ToString();

        // Act
        Reward invalidId() => new("");
        Reward invalidAmount() => new(id, amount: 0);
        Reward invalidName() => new(id, name: "");
        Reward valid() => new(id);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should throw and exception");
        Assert.ThrowsException<ArgumentException>(invalidAmount, "An invalid amount should throw an exception");
        Assert.ThrowsException<ArgumentException>(invalidName, "An invalid name should throw an exception");
        Assert.IsInstanceOfType(valid(), typeof(Reward), "A valid constructor should successfully instantiate the class");
    }

    [TestMethod]
    public void SetAmountTest()
    {
        // Arrange
        Reward reward = new(SnowflakeService.Generate().ToString());

        // Act
        int amountBeforeSet = reward.Amount;

        bool validWorked = reward.SetAmount(10);
        bool invalidWorked = reward.SetAmount(0);

        int amountAfterSet = reward.Amount;

        // Assert
        Assert.AreEqual(1, amountBeforeSet, "The amount should be the default");
        Assert.IsTrue(validWorked, "A valid amount should work");
        Assert.IsFalse(invalidWorked, "An invalid amount should not work");
        Assert.AreEqual(10, amountAfterSet, "The amount should be updated to the latest valid call");
    }

    [TestMethod]
    public void SetNameTest()
    {
        // Arrange
        Reward reward = new(SnowflakeService.Generate().ToString());

        // Act
        bool nullNameWorked = reward.SetName(null);
        bool validWorked = reward.SetName("Some Title");
        bool invalidWorked = reward.SetName("");

        string? nameAfterSet = reward.Name;

        // Assert
        Assert.IsTrue(nullNameWorked, "A null name should work");
        Assert.IsTrue(validWorked, "A valid name should work");
        Assert.IsFalse(invalidWorked, "An invalid name should not work");
        Assert.AreEqual("Some Title", nameAfterSet, "The name should match the most recent valid call");
    }
}
