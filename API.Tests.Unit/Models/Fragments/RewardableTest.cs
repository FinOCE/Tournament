namespace API.Tests.Unit.Models.Fragments;

[TestClass]
public class RewardableTest
{
    SnowflakeService SnowflakeService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
    }

    [TestMethod]
    public void SetPrizesTest()
    {
        // Arrange
        Rewardable rewardable = new();

        Dictionary<string, Prize> prizes = new();
        for (int i = 0; i < 3; i++)
        {
            IPlacement placement = new ExactPlacement(i + 1);
            Reward reward = new(SnowflakeService.Generate().ToString());
            Prize prize = new(SnowflakeService.Generate().ToString(), placement, reward);
            prizes.Add(prize.Id, prize);
        }

        // Act
        int prizesBeforeSet = rewardable.Prizes.Count;

        rewardable.SetPrizes(prizes);

        int prizesAfterSet = rewardable.Prizes.Count;

        // Assert
        Assert.AreEqual(0, prizesBeforeSet, "The class should initialise with no prizes");
        Assert.AreEqual(3, prizesAfterSet, "All of the prizes should have been set");

        foreach (string id in prizes.Keys)
            Assert.IsTrue(prizes.ContainsKey(id), "All prize IDs should exist in the dictionary");
    }

    [TestMethod]
    public void AddPrizeTest()
    {
        // Arrange
        Rewardable rewardable = new();

        Dictionary<string, Prize> prizes = new();
        for (int i = 0; i < 3; i++)
        {
            IPlacement placement = new ExactPlacement(i + 1);
            Reward reward = new(SnowflakeService.Generate().ToString());
            Prize prize = new(SnowflakeService.Generate().ToString(), placement, reward);
            prizes.Add(prize.Id, prize);
        }

        int[] prizeCount = new int[prizes.Count];
        int index = rewardable.Prizes.Count;

        // Act
        int prizesBeforeAdd = rewardable.Prizes.Count;

        foreach (Prize prize in prizes.Values)
        {
            bool added = rewardable.AddPrize(prize);
            if (added)
                prizeCount[index++] = rewardable.Prizes.Count;
        }

        bool duplicateWorked = rewardable.AddPrize(prizes.Values.First());

        int prizesAfterAdd = rewardable.Prizes.Count;

        // Assert
        Assert.AreEqual(0, prizesBeforeAdd, "The class should initialise with no prizes");
        Assert.AreEqual(3, prizesAfterAdd, "All of the prizes should have been added");
        Assert.IsFalse(duplicateWorked, "Duplicates should not be able to be added");

        int prev = -1;
        foreach (int count in prizeCount)
        {
            Assert.IsTrue(count > prev, "The count should increase every time a new prize is added");
            prev++;
        }
            

        foreach (string id in prizes.Keys)
            Assert.IsTrue(prizes.ContainsKey(id), "All prize IDs should exist in the dictionary");
    }

    [TestMethod]
    public void RemovePrizeTest()
    {
        // Arrange
        Rewardable rewardable = new();

        Dictionary<string, Prize> prizes = new();
        for (int i = 0; i < 3; i++)
        {
            IPlacement placement = new ExactPlacement(i + 1);
            Reward reward = new(SnowflakeService.Generate().ToString());
            Prize prize = new(SnowflakeService.Generate().ToString(), placement, reward);
            prizes.Add(prize.Id, prize);
            rewardable.AddPrize(prize);
        }

        int[] prizeCount = new int[prizes.Count];
        int index = 0;

        // Act
        int prizesBeforeRemove = rewardable.Prizes.Count;

        foreach (Prize prize in prizes.Values)
        {
            bool removed = rewardable.RemovePrize(prize.Id);
            if (removed)
                prizeCount[index++] = rewardable.Prizes.Count;
        }

        bool duplicateWorked = rewardable.RemovePrize(prizes.Values.First().Id);

        int prizesAfterRemove = rewardable.Prizes.Count;

        // Assert
        Assert.AreEqual(3, prizesBeforeRemove, "The class should have 3 prizes added");
        Assert.AreEqual(0, prizesAfterRemove, "All of the prizes should have been removed");
        Assert.IsFalse(duplicateWorked, "Duplicates removal requests should not be allowed");

        int prev = prizes.Count;
        foreach (int count in prizeCount)
        {
            Assert.IsTrue(count < prev, "The count should decrease every time a new prize is removed");
            prev--;
        }
    }
}
