namespace API.Tests.Unit.Models.Events.Prizes;

[TestClass]
public class PrizeTest
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
        IPlacement placement = new ExactPlacement(1);
        Reward reward = new(SnowflakeService.Generate().ToString());

        // Act
        Prize invalidId() => new("", placement, reward);
        Prize valid() => new(SnowflakeService.Generate().ToString(), placement, reward);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should throw an error");
        Assert.IsInstanceOfType(valid(), typeof(Prize), "A valid call should instantiate the class");
    }

    [TestMethod]
    public void SetRewardTest()
    {
        // Arrange
        IPlacement placement = new ExactPlacement(1);
        Reward reward = new(SnowflakeService.Generate().ToString());
        Prize prize = new(SnowflakeService.Generate().ToString(), placement, reward);

        // Act
        int amountBeforeSet = prize.Reward.Amount;

        prize.SetReward(new(SnowflakeService.Generate().ToString(), 10));

        int amountAfterSet = prize.Reward.Amount;

        // Assert
        Assert.AreEqual(1, amountBeforeSet, "The reward should be what was initially set");
        Assert.AreEqual(10, amountAfterSet, "The reward should have successfully updated");
    }

    [TestMethod]
    public void CompareToTest()
    {
        // Arrange
        Prize[] prizes = new Prize[]
        {
            new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(1),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(2),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(3),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(4),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new GroupPlacement(1),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new GroupPlacement(2),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new GroupPlacement(3),
                new(SnowflakeService.Generate().ToString())),
            new(
                SnowflakeService.Generate().ToString(),
                new GroupPlacement(4),
                new(SnowflakeService.Generate().ToString()))
        };

        // Act
        Array.Sort(prizes);

        // Assert
        Assert.IsTrue(
            prizes[0].Placement.Position == 1 &&
            prizes[0].Placement.GetType() == typeof(ExactPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[1].Placement.Position == 1 &&
            prizes[1].Placement.GetType() == typeof(GroupPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[2].Placement.Position == 2 &&
            prizes[2].Placement.GetType() == typeof(ExactPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[3].Placement.Position == 2 &&
            prizes[3].Placement.GetType() == typeof(GroupPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[4].Placement.Position == 3 &&
            prizes[4].Placement.GetType() == typeof(ExactPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[5].Placement.Position == 3 &&
            prizes[5].Placement.GetType() == typeof(GroupPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[6].Placement.Position == 4 &&
            prizes[6].Placement.GetType() == typeof(ExactPlacement),
            "The sorting order should prioritise position then type");
        Assert.IsTrue(
            prizes[7].Placement.Position == 4 &&
            prizes[7].Placement.GetType() == typeof(GroupPlacement),
            "The sorting order should prioritise position then type");
    }
}
