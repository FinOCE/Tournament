namespace API.Tests.Unit.Models.Events;

[TestClass]
public class TournamentTest
{
    SnowflakeService SnowflakeService = null!;
    Tournament Tournament = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();
        Tournament = new(SnowflakeService.Generate().ToString(), "Tournament");
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Arrange
        string sg() => SnowflakeService.Generate().ToString(); // Shorten since it's being used a lot

        // Act
        Tournament invalidId() => new("", "Valid");
        Tournament invalidName() => new(sg(), "12");
        Tournament invalidDescription() => new(sg(), "Name", description: ":|");
        Tournament invalidRules() => new(sg(), "Name", rules: ":|");
        Tournament invalidIcon() => new(sg(), "Name", icon: "12345678901234567");
        Tournament invalidBanner() => new(sg(), "Name", banner: "12345678901234567");
        Tournament valid() => new(sg(), "Name", "Description", "Rules", null, null, null);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should throw exception");
        Assert.ThrowsException<ArgumentException>(invalidName, "An invalid name should throw exception");
        Assert.ThrowsException<ArgumentException>(invalidDescription, "An invalid description should throw exception");
        Assert.ThrowsException<ArgumentException>(invalidRules, "An invalid rules should throw exception");
        Assert.ThrowsException<ArgumentException>(invalidIcon, "An invalid icon should throw exception");
        Assert.ThrowsException<ArgumentException>(invalidBanner, "An invalid banner should throw exception");
        Assert.IsInstanceOfType(valid(), typeof(Tournament), "Valid paramaters should instantiate the class");
    }

    [TestMethod]
    public void SetNameTest()
    {
        // Arrange
        string tooShortName = "12";
        string tooLongName = "123456789012345678901234567890123";
        string invalidCharactersName = "This will not work :)";
        string invalidAftertrimName = "      12      ";
        string validName1 = "This will work";
        string validName2 = "123";
        string validName3 = "Using-Some-Dashes";
        string validName4 = "    spaces will be removed           ";
        string validName5 = "...Dots are okay...";

        // Act
        bool tooShortNameWorked = Tournament.SetName(tooShortName);
        bool tooLongNameWorked = Tournament.SetName(tooLongName);
        bool invalidCharactersNameWorked = Tournament.SetName(invalidCharactersName);
        bool invalidAfterTrimNameWorked = Tournament.SetName(invalidAftertrimName);

        string nameBeforeExpectedValids = Tournament.Name;

        bool validName1Worked = Tournament.SetName(validName1);
        bool validName2Worked = Tournament.SetName(validName2);
        bool validName3Worked = Tournament.SetName(validName3);
        bool validName4Worked = Tournament.SetName(validName4);
        bool validName5Worked = Tournament.SetName(validName5);

        // Assert
        Assert.IsFalse(tooShortNameWorked, "A name that is too short shouldn't work");
        Assert.IsFalse(tooLongNameWorked, "A name that is too long shouldn't work");
        Assert.IsFalse(invalidCharactersNameWorked, "A name with invalid characters shouldn't work");
        Assert.IsFalse(invalidAfterTrimNameWorked, "An invalid name with lot's of padding spaces shouldn't work");
        Assert.IsTrue(validName1Worked, "This name should be working");
        Assert.IsTrue(validName2Worked, "This name should be working");
        Assert.IsTrue(validName3Worked, "This name should be working");
        Assert.IsTrue(validName4Worked, "This name should be working");
        Assert.IsTrue(validName5Worked, "This name should be working");
        Assert.AreEqual("Tournament", nameBeforeExpectedValids, "The name should not have changed from invalid names");
        Assert.AreEqual(validName5, Tournament.Name, "Successful changes should update the name");
    }

    [TestMethod]
    public void SetDescriptionTest()
    {
        // Arrange
        string tooLongDescription = string.Join("", new int[2049]);
        string invalidCharactersDescription = "This contains some invalid charaters... Oh well!";
        string validDescription = "This should be a valid description. It will hopefully look correct.";

        // Act
        bool tooLongDescriptionWorked = Tournament.SetDescription(tooLongDescription);
        bool invalidCharactersDescriptionWorked = Tournament.SetDescription(invalidCharactersDescription);

        string? descriptionBeforeExpectedValids = Tournament.Description;
        
        bool nullDescriptionWorked = Tournament.SetDescription(null);
        bool validDescriptionWorked = Tournament.SetDescription(validDescription);

        // Assert
        Assert.IsFalse(tooLongDescriptionWorked, "A description that is too long shoulnd't work");
        Assert.IsFalse(invalidCharactersDescriptionWorked, "A description containing invalid characters shouldn't work");
        Assert.IsTrue(nullDescriptionWorked, "A null value should work");
        Assert.IsTrue(validDescriptionWorked, "This description should be working");
        Assert.AreEqual(null, descriptionBeforeExpectedValids, "The description should not have changed from invalid descriptions");
        Assert.AreEqual(validDescription, Tournament.Description, "Successful changes should update the description");
    }

    [TestMethod]
    public void SetRulesTest()
    {
        // Arrange
        string tooLongRules = string.Join("", new int[2049]);
        string invalidCharactersRules = "This contains some invalid charaters... Oh well!";
        string validRules = "This should be a valid rules body. It will hopefully look correct.";

        // Act
        bool tooLongRulesWorked = Tournament.SetRules(tooLongRules);
        bool invalidCharactersRulesWorked = Tournament.SetRules(invalidCharactersRules);

        string? rulesBeforeExpectedValids = Tournament.Rules;

        bool nullRulesWorked = Tournament.SetRules(null);
        bool validRulesWorked = Tournament.SetRules(validRules);

        // Assert
        Assert.IsFalse(tooLongRulesWorked, "A rules body that is too long shoulnd't work");
        Assert.IsFalse(invalidCharactersRulesWorked, "A rules body containing invalid characters shouldn't work");
        Assert.IsTrue(nullRulesWorked, "A null value should work");
        Assert.IsTrue(validRulesWorked, "This rules body should be working");
        Assert.AreEqual(null, rulesBeforeExpectedValids, "The rules should not have changed from invalid rules");
        Assert.AreEqual(validRules, Tournament.Rules, "Successful changes should update the rules");
    }

    [TestMethod]
    public void AddCoordinatorTest()
    {
        // Arrange
        Coordinator[] coordinators = new Coordinator[5];
        for (int i = 0; i < coordinators.Length; i++)
        {
            User user = new(SnowflakeService.Generate().ToString(), "Coordinator", i + 1);
            Coordinator coordinator = new(user, Tournament);
            coordinators[i] = coordinator;
        }

        // Act
        int countBeforeAdding = Tournament.Coordinators.Count;

        foreach (Coordinator coordinator in coordinators)
            Tournament.AddCoordinator(coordinator);

        bool duplicateAddWorked = Tournament.AddCoordinator(coordinators[0]);

        int countAfterAdding = Tournament.Coordinators.Count;

        // Assert
        Assert.AreEqual(0, countBeforeAdding, "There should be no coordinators to start");
        Assert.IsFalse(duplicateAddWorked, "Duplicate coordinators should not be able to be added");
        Assert.AreEqual(5, countAfterAdding, "A total of 5 coordinators should be added");

        foreach (Coordinator coordinator in coordinators)
            Assert.IsTrue(
                Tournament.Coordinators.ContainsKey(coordinator.User.Id),
                "All coordinators should be included in the tournament");
    }

    [TestMethod]
    public void RemoveCoordinatorTest()
    {
        // Arrange
        Coordinator[] coordinators = new Coordinator[5];
        for (int i = 0; i < coordinators.Length; i++)
        {
            User user = new(SnowflakeService.Generate().ToString(), "Coordinator", i + 1);
            Coordinator coordinator = new(user, Tournament);
            coordinators[i] = coordinator;
        }

        foreach (Coordinator coordinator in coordinators)
            Tournament.AddCoordinator(coordinator);

        // Act
        int countBeforeRemoving = Tournament.Coordinators.Count;

        foreach (Coordinator coordinator in coordinators)
            Tournament.RemoveCoordinator(coordinator.User.Id);

        bool duplicateRemoveWorked = Tournament.RemoveCoordinator(coordinators[0].User.Id);

        int countAfterRemoving = Tournament.Coordinators.Count;

        // Assert
        Assert.AreEqual(5, countBeforeRemoving, "There should be 5 coordinators to start");
        Assert.IsFalse(duplicateRemoveWorked, "Duplicate removals should not be successful");
        Assert.AreEqual(0, countAfterRemoving, "There should be no coordinators at the end");
    }

    [TestMethod]
    public void SetIconTest()
    {
        // Arrange
        string wrongLengthIcon = "12345678901234567";
        string invalidCharactersIcon = "123456789012345.";
        string validIcon = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthIconWorked = Tournament.SetIcon(wrongLengthIcon);
        bool invalidCharactersIconWorked = Tournament.SetIcon(invalidCharactersIcon);

        string? iconBeforeExpectedValids = Tournament.Icon;

        bool nullIconWorked = Tournament.SetIcon(null);
        bool validIconWorked = Tournament.SetIcon(validIcon);

        // Assert
        Assert.IsFalse(wrongLengthIconWorked, "An icon of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersIconWorked, "An icon with invalid characters shouldn't work");
        Assert.IsTrue(nullIconWorked, "A null value should work");
        Assert.IsTrue(validIconWorked, "This icon should be working");
        Assert.AreEqual(Tournament.DefaultIcon, iconBeforeExpectedValids, "The icon should not have changed from invalid icons");
        Assert.AreEqual(validIcon, Tournament.Icon, "Successful changes should update the icon");
    }

    [TestMethod]
    public void SetBannerTest()
    {
        // Arrange
        string wrongLengthBanner = "12345678901234567";
        string invalidCharactersBanner = "123456789012345.";
        string validBanner = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthBannerWorked = Tournament.SetBanner(wrongLengthBanner);
        bool invalidCharactersBannerWorked = Tournament.SetBanner(invalidCharactersBanner);

        string? bannerBeforeExpectedValids = Tournament.Banner;

        bool nullBannerWorked = Tournament.SetBanner(null);
        bool validBannerWorked = Tournament.SetBanner(validBanner);

        // Assert
        Assert.IsFalse(wrongLengthBannerWorked, "An banner of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersBannerWorked, "An banner with invalid characters shouldn't work");
        Assert.IsTrue(nullBannerWorked, "A null value should work");
        Assert.IsTrue(validBannerWorked, "This banner should be working");
        Assert.AreEqual(Tournament.DefaultBanner, bannerBeforeExpectedValids, "The banner should not have changed from invalid banners");
        Assert.AreEqual(validBanner, Tournament.Banner, "Successful changes should update the banner");
    }

    [TestMethod]
    public void SetGameTest()
    {
        // Arrange
        Game game = new(SnowflakeService.Generate().ToString(), "Game");

        // Act
        Game? oldGame = Tournament.Game;

        Tournament.SetGame(game);

        Game? newGame = Tournament.Game;

        // Assert
        Assert.IsNull(oldGame, "The tournament should be instantiated without a game");
        Assert.AreEqual(game.Id, newGame!.Id, "The game should have been properly set");
    }

    [TestMethod]
    public void AddPrizeTest()
    {
        // Arrange
        Prize[] prizes = new Prize[4];
        for (int i = 1; i <= prizes.Length; i++)
            prizes[i - 1] = new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(i),
                new(SnowflakeService.Generate().ToString(), 1000 / i));

        // Act
        int countBeforeAdding = Tournament.Prizes.Count;

        for (int i = 0; i < prizes.Length; i++)
            Tournament.AddPrize(prizes[i]);

        bool duplicateAddWorked = Tournament.AddPrize(prizes[0]);

        int countAfterAdding = Tournament.Prizes.Count;

        // Assert
        Assert.AreEqual(0, countBeforeAdding, "There should be no prizes to start");
        Assert.IsFalse(duplicateAddWorked, "Duplicate prizes should not be able to be added");
        Assert.AreEqual(4, countAfterAdding, "A total of 5 prizes should be added");

        foreach (Prize prize in prizes)
            Assert.IsTrue(
                Tournament.Prizes.ContainsKey(prize.Id),
                "All prizes should be included in the tournament");
    }

    [TestMethod]
    public void RemovePrizeTest()
    {
        // Arrange
        Prize[] prizes = new Prize[4];
        for (int i = 1; i <= prizes.Length; i++)
            prizes[i - 1] = new(
                SnowflakeService.Generate().ToString(),
                new ExactPlacement(i),
                new(SnowflakeService.Generate().ToString(), 1000 / i));

        for (int i = 0; i < prizes.Length; i++)
            Tournament.AddPrize(prizes[i]);

        // Act
        int countBeforeRemoving = Tournament.Prizes.Count;

        foreach (Prize prize in prizes)
            Tournament.RemovePrize(prize.Id);

        bool duplicateRemoveWorked = Tournament.RemovePrize(prizes[0].Id);

        int countAfterRemoving = Tournament.Prizes.Count;

        // Assert
        Assert.AreEqual(4, countBeforeRemoving, "There should be 4 prizes to start");
        Assert.IsFalse(duplicateRemoveWorked, "Duplicate removals should not be successful");
        Assert.AreEqual(0, countAfterRemoving, "There should be no prizes at the end");
    }

    [TestMethod]
    [Ignore]
    public void AddEventTest()
    {
    }

    [TestMethod]
    [Ignore]
    public void RemoveEventTest()
    {
    }
}
