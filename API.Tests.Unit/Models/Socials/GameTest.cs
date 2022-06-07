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
    public void AddCategoryTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");

        string tooLongBeforeTrimCategory = "        12345        ";
        string validCategory = "Racing/Driving";

        // Act
        int categoryCountBeforeAdding = game.Categories.Length;

        bool tooShortCategoryWorked = game.AddCategory("");
        bool onlySpacesCategoryWorked = game.AddCategory("    ");
        bool tooLongCategoryWorked = game.AddCategory("12345678901234567");
        bool tooLongBeforeTrimCategoryWorked = game.AddCategory(tooLongBeforeTrimCategory);
        bool validCategoryWorked = game.AddCategory(validCategory);
        bool duplicateWorked = game.AddCategory(validCategory);

        int categoryCountAfterAdding = game.Categories.Length;

        // Assert
        Assert.AreEqual(0, categoryCountBeforeAdding, "The game should instantiate with no categories");
        Assert.IsFalse(tooShortCategoryWorked, "A category that is too short shouldn't work");
        Assert.IsFalse(onlySpacesCategoryWorked, "A category that is only spaces shouldn't work");
        Assert.IsFalse(tooLongCategoryWorked, "A category that is too long shouldn't work");
        Assert.IsTrue(tooLongBeforeTrimCategoryWorked, "A category that once trimming is the right length should work");
        Assert.IsTrue(validCategoryWorked, "An expected valid category should work");
        Assert.IsFalse(duplicateWorked, "A duplicate category should not be added");
        Assert.AreEqual(2, categoryCountAfterAdding, "After running calls there should be 2 categories");
    }

    [TestMethod]
    public void RemoveCategoryTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");
        game.AddCategory("Racing");
        game.AddCategory("Driving");
        game.AddCategory("Competitive");

        // Act
        int categoryCountBeforeRemoval = game.Categories.Length;
        bool containsRacingBeforeRemoval = game.Categories.Contains("Racing");
        bool removedRacing = game.RemoveCategory("Racing");
        bool duplicateRemoval = game.RemoveCategory("Racing");
        bool containsRacingAfterRemoval = game.Categories.Contains("Racing");
        int categoryCountAfterRemoval = game.Categories.Length;

        bool removedDriving = game.RemoveCategory("Driving");
        bool removedCompetitive = game.RemoveCategory("Competitive");
        int categoryCountAfterAllRemoval = game.Categories.Length;

        // Assert
        Assert.AreEqual(3, categoryCountBeforeRemoval, "The test should start with 3 categories");
        Assert.IsTrue(containsRacingBeforeRemoval, "The Racing category should exist");
        Assert.IsTrue(removedRacing, "Racing should be succesfully removed");
        Assert.IsFalse(duplicateRemoval, "Duplicate removal should not be successful");
        Assert.IsFalse(containsRacingAfterRemoval, "The Racing category should not exist after removal");
        Assert.AreEqual(2, categoryCountAfterRemoval, "There should be 2 remaining categories");
        Assert.IsTrue(removedDriving, "Driving should be successfully removed");
        Assert.IsTrue(removedCompetitive, "Competitive should be successfully removed");
        Assert.AreEqual(0, categoryCountAfterAllRemoval, "There should be no remaining categories");
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
    public void SetIconTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");

        string wrongLengthIcon = "12345678901234567";
        string invalidCharactersIcon = "123456789012345.";
        string validIcon = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthIconWorked = game.SetIcon(wrongLengthIcon);
        bool invalidCharactersIconWorked = game.SetIcon(invalidCharactersIcon);

        string? iconBeforeExpectedValids = game.Icon;

        bool nullIconWorked = game.SetIcon(null);
        bool validIconWorked = game.SetIcon(validIcon);

        // Assert
        Assert.IsFalse(wrongLengthIconWorked, "An icon of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersIconWorked, "An icon with invalid characters shouldn't work");
        Assert.IsTrue(nullIconWorked, "A null value should work");
        Assert.IsTrue(validIconWorked, "This icon should be working");
        Assert.AreEqual(Game.DefaultIcon, iconBeforeExpectedValids, "The icon should not have changed from invalid icons");
        Assert.AreEqual(validIcon, game.Icon, "Successful changes should update the icon");
    }

    [TestMethod]
    public void SetBannerTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Some Game");

        string wrongLengthBanner = "12345678901234567";
        string invalidCharactersBanner = "123456789012345.";
        string validBanner = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthBannerWorked = game.SetBanner(wrongLengthBanner);
        bool invalidCharactersBannerWorked = game.SetBanner(invalidCharactersBanner);

        string? bannerBeforeExpectedValids = game.Banner;

        bool nullBannerWorked = game.SetBanner(null);
        bool validBannerWorked = game.SetBanner(validBanner);

        // Assert
        Assert.IsFalse(wrongLengthBannerWorked, "An banner of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersBannerWorked, "An banner with invalid characters shouldn't work");
        Assert.IsTrue(nullBannerWorked, "A null value should work");
        Assert.IsTrue(validBannerWorked, "This banner should be working");
        Assert.AreEqual(Game.DefaultBanner, bannerBeforeExpectedValids, "The banner should not have changed from invalid banners");
        Assert.AreEqual(validBanner, game.Banner, "Successful changes should update the banner");
    }
}
