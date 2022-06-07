namespace API.Tests.Unit.Models.Socials;

[TestClass]
public class GameTest
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
        string sg() => SnowflakeService.Generate().ToString(); // Because it's used a lot

        // Act
        Game invalidId() => new("", "Game");
        Game invalidName() => new(sg(), "");
        Game invalidCategories() => new(sg(), "Game", categories: new string[] { "" });
        Game invalidDescription() => new(sg(), "Game", description: "");
        Game invalidIcon() => new(sg(), "Game", icon: "invalid icon");
        Game invalidBanner() => new(sg(), "Game", banner: "invalid banner");
        Game valid() => new(sg(), "Game");

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not be allowed");
        Assert.ThrowsException<ArgumentException>(invalidName, "An invalid name should not be allowed");
        Assert.ThrowsException<ArgumentException>(invalidCategories, "Invalid categories should not be allowed");
        Assert.ThrowsException<ArgumentException>(invalidDescription, "An invalid description should not be allowed");
        Assert.ThrowsException<ArgumentException>(invalidIcon, "An invalid icon should not be allowed");
        Assert.ThrowsException<ArgumentException>(invalidBanner, "An invalid banner should not be allowed");
        Assert.IsInstanceOfType(valid(), typeof(Game), "A game should be successfully instantiated");
    }

    [TestMethod]
    public void SetNameTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");

        string[] invalid = new[]
        {
            "12345678901234567890123456789012345678901234567890123456789012345",
            "",
            "        "
        };

        string[] valid = new[]
        {
            "Counter Strike: Global Offensive",
            "Clash Royale",
            "Rocket League Sideswipe"
        };

        // Act
        bool[] invalidSuccess = invalid.Select(n => game.SetName(n)).ToArray();
        string nameAfterInvalid = game.Name;

        bool[] validSuccess = valid.Select(n => game.SetName(n)).ToArray();
        string nameAfterValid = game.Name;

        // Assert
        foreach (bool success in invalidSuccess)
            Assert.IsFalse(success, "Invalid names should not be allowed");

        foreach (bool success in validSuccess)
            Assert.IsTrue(success, "Valid names should be allowed");

        Assert.AreEqual("Some Game", nameAfterInvalid, "Invalid tests should not have updated the name");
        Assert.AreEqual(valid.Last(), game.Name, "The name should be the last valid tested");
    }

    [TestMethod]
    [Ignore]
    public void AddCategoryTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public void RemoveCategoryTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    public void SetDescriptionTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");

        string[] invalid = new[]
        {
            "",
            "        ",
            string.Join("", new int[2049])
        };

        string?[] valid = new[]
        {
            "This is a valid description!",
            "This is also valid.",
            "All of these should work: !@#$%^&*()_+-={}[]|\\:\";'<>,./? and so on...",
            "Numbers should also be ok! 1234567890",
            "       Basically anything should be ok            ",
            null
        };

        // Act
        bool[] invalidSuccess = invalid.Select(d => game.SetDescription(d)).ToArray();
        string? descriptionAfterInvalid = game.Description;

        bool[] validSuccess = valid.Select(d => game.SetDescription(d)).ToArray();
        string? descriptionAfterValid = game.Description;

        // Assert
        foreach (bool success in invalidSuccess)
            Assert.IsFalse(success, "Invalid descriptions should not be allowed");

        foreach (bool success in validSuccess)
            Assert.IsTrue(success, "Valid descriptions should be allowed");

        Assert.AreEqual(null, descriptionAfterInvalid, "Invalid tests should not have updated the description");
        Assert.AreEqual(valid.Last()?.Trim(), game.Description, "The description should be the last valid tested (and trimmed)");
    }

    [TestMethod]
    [Ignore]
    public void SetIconTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public void SetBannerTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public void AddSocialTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public void RemoveSocialTest()
    {
        // Arrange


        // Act


        // Assert

    }

    [TestMethod]
    [Ignore]
    public void VerifyTest()
    {
        // Arrange


        // Act


        // Assert

    }
}
