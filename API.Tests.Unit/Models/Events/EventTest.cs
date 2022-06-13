namespace API.Tests.Unit.Models.Events;

[TestClass]
public class EventTest
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
    public void FinishedTest()
    {
        // Arrange
        Event ev = new(SnowflakeService.Generate().ToString(), "Event", Tournament, DateTime.UtcNow);

        Dictionary<string, ITeam> teams = new();
        Dictionary<string, int> seeds = new();
        Team team = new(SnowflakeService.Generate().ToString(), "Team");
        teams.Add(team.Id, team);
        seeds.Add(team.Id, 1);

        IBracketBuilder builder = new SingleEliminationBuilder(
            SnowflakeService.Generate().ToString(),
            SnowflakeService,
            teams,
            seeds,
            1);
        builder.Generate();

        ev.AddBracket(builder);

        // Act
        bool finishedBefore = ev.Finished;
        bool firstCall = ev.Finish();
        bool secondCall = ev.Finish();
        bool finishedAfter = ev.Finished;

        // Assert
        Assert.IsFalse(finishedBefore, "Events should not start off finished");
        Assert.IsTrue(firstCall, "The tournament should be successfully finished");
        Assert.IsFalse(secondCall, "Duplicate calls should not be successful");
        Assert.IsTrue(finishedAfter, "The tournament should be marked as finished");
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Arrange
        string sg() => SnowflakeService.Generate().ToString(); // Shorten since it's being used a lot

        Dictionary<string, ITeam> teams = new();
        Dictionary<string, int> seeds = new();
        Team team = new(SnowflakeService.Generate().ToString(), "Team");
        teams.Add(team.Id, team);
        seeds.Add(team.Id, 1);

        IBracketBuilder builder = new SingleEliminationBuilder(
            SnowflakeService.Generate().ToString(),
            SnowflakeService,
            teams,
            seeds,
            1);
        builder.Generate();

        Dictionary<string, IBracketBuilder> brackets = new();
        brackets.Add(builder.Id, builder);

        // Act
        Event invalidId() => new("", "Event", Tournament, DateTime.UtcNow);
        Event invalidName() => new(sg(), "", Tournament, DateTime.UtcNow);
        Event invalidRegTime() => new(
            sg(),
            "Event",
            Tournament,
            DateTime.UtcNow,
            registrationTimestamp: DateTime.UtcNow.AddMinutes(10));
        Event invalidFinishTime() => new(
            sg(),
            "Event",
            Tournament,
            DateTime.UtcNow,
            brackets: brackets,
            finishedTimestamp: DateTime.UtcNow.AddMinutes(-10));
        Event valid() => new(sg(), "Event", Tournament, DateTime.UtcNow);

        // TODO: Add test where finishedTimestamp is valid but no completed bracket is provided

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not work");
        Assert.ThrowsException<ArgumentException>(invalidName, "An invalid name should not work");
        Assert.ThrowsException<ArgumentException>(invalidRegTime, "An invalid registration time should not work");
        Assert.ThrowsException<ArgumentException>(invalidFinishTime, "An invalid finish time should not work");
        Assert.IsInstanceOfType(valid(), typeof(Event), "A valid constructor should work");
    }

    [TestMethod]
    public void SetNameTest()
    {
        // Arrange
        Event ev = new(SnowflakeService.Generate().ToString(), "Event", Tournament, DateTime.UtcNow);

        // Act
        string nameBeforeInvalid = ev.Name;

        bool tooShort = ev.SetName("");
        bool tooLong = ev.SetName(string.Join('.', new int[65]));

        string nameAfterInvalid = ev.Name;

        bool valid1 = ev.SetName("Example's 1v1 Tournament");
        bool valid2 = ev.SetName("Tournament: Event: Season Exciting!");
        bool valid3 = ev.SetName("     Event!     ");

        string nameAfterValid = ev.Name;

        // Assert
        Assert.AreEqual("Event", nameBeforeInvalid, "The name should start as what it is defined as");
        Assert.IsFalse(tooShort, "A name that is too short should not be valid");
        Assert.IsFalse(tooLong, "A name that is too long should not be valid");
        Assert.AreEqual("Event", nameAfterInvalid, "The name should not have changed from invalid calls");
        Assert.IsTrue(valid1, "A valid call should work");
        Assert.IsTrue(valid2, "A valid call should work");
        Assert.IsTrue(valid3, "A valid call should work");
        Assert.AreEqual("Event!", nameAfterValid, "The name should be the same as the latest valid call and trimmed");
    }

    [TestMethod]
    public void SetStartTimestampTest()
    {
        // Arrange
        Event ev1 = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        Event ev2 = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            new DateTime(2222, 1, 1, 2, 0, 0),
            registrationTimestamp: new DateTime(2222, 1, 1, 1, 0, 0));

        // Act
        bool alreadyStarted = ev1.SetStartTimestamp(new DateTime(2222, 1, 1, 0, 0, 0));

        bool changesRego = ev2.SetStartTimestamp(new DateTime(2222, 1, 1, 0, 30, 0));
        DateTime startTimeAfterChange = ev2.StartTimestamp;
        DateTime regoTimeAfterChange = ev2.RegistrationTimestamp;

        bool changesStart = ev2.SetStartTimestamp(new DateTime(2222, 1, 1, 3, 0, 0));
        DateTime startTimeAfterChangeStartOnly = ev2.StartTimestamp;
        DateTime regoTimeAfterChangeStartOnly = ev2.RegistrationTimestamp;

        // Assert
        Assert.IsFalse(alreadyStarted, "Trying to change the start time after it has started should not work");
        Assert.IsTrue(changesRego, "Changing the time to any time in the future should work");
        Assert.AreEqual(new DateTime(2222, 1, 1, 0, 30, 0), startTimeAfterChange, "Start time should be updated");
        Assert.AreEqual(new DateTime(2222, 1, 1, 0, 30, 0), regoTimeAfterChange, "Rego time should be updated");
        Assert.IsTrue(changesStart, "Changing the time should work but also not change the rego time");
        Assert.AreEqual(new DateTime(2222, 1, 1, 3, 0, 0), startTimeAfterChangeStartOnly, "Start time should be updated");
        Assert.AreEqual(new DateTime(2222, 1, 1, 0, 30, 0), regoTimeAfterChangeStartOnly, "Rego time should remain the same");
    }

    [TestMethod]
    public void SetRegistrationTimestampTest()
    {
        // Arrange
        Event ev1 = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        Event ev2 = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            new DateTime(2222, 1, 1, 2, 0, 0),
            registrationTimestamp: new DateTime(2222, 1, 1, 1, 0, 0));

        // Act
        bool alreadyStarted = ev1.SetRegistrationTimestamp(new DateTime(2222, 1, 1, 0, 0, 0));

        bool changesRego = ev2.SetRegistrationTimestamp(new DateTime(2222, 1, 1, 0, 30, 0));
        DateTime startTimeAfterChange = ev2.StartTimestamp;
        DateTime regoTimeAfterChange = ev2.RegistrationTimestamp;

        bool changesStart = ev2.SetRegistrationTimestamp(new DateTime(2222, 1, 1, 3, 0, 0));
        DateTime startTimeAfterChangeStartOnly = ev2.StartTimestamp;
        DateTime regoTimeAfterChangeStartOnly = ev2.RegistrationTimestamp;

        // Assert
        Assert.IsFalse(alreadyStarted, "Trying to change the start time after it has started should not work");
        Assert.IsTrue(changesRego, "Changing the time back should update the rego time but not the start time");
        Assert.AreEqual(new DateTime(2222, 1, 1, 2, 0, 0), startTimeAfterChange, "Start time should remain the same");
        Assert.AreEqual(new DateTime(2222, 1, 1, 0, 30, 0), regoTimeAfterChange, "Rego time should be updated");
        Assert.IsTrue(changesStart, "Changing the time forward should update both the rego and start time");
        Assert.AreEqual(new DateTime(2222, 1, 1, 3, 0, 0), startTimeAfterChangeStartOnly, "Start time should be updated");
        Assert.AreEqual(new DateTime(2222, 1, 1, 3, 0, 0), regoTimeAfterChangeStartOnly, "Rego time should be updated");
    }

    [TestMethod]
    public void AddBracketTest()
    {
        // Arrange
        Event ev = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        IBracketBuilder[] builders = new IBracketBuilder[3];
        for (int i = 0; i < builders.Length; i++)
        {
            IBracketBuilder builder = new SingleEliminationBuilder(
                SnowflakeService.Generate().ToString(),
                SnowflakeService);

            builders[i] = builder;
        }

        // Act
        int bracketsBefore = ev.Brackets.Count;

        bool anyFailed = false;
        foreach (IBracketBuilder builder in builders)
        {
            bool worked = ev.AddBracket(builder);

            if (!worked)
                anyFailed = false;
        }

        bool duplicateWorked = ev.AddBracket(builders[0]);

        int bracketsAfter = ev.Brackets.Count;

        // Assert
        Assert.AreEqual(0, bracketsBefore, "The event should start with no brackets");
        Assert.IsFalse(anyFailed, "No valid builders should fail");
        Assert.IsFalse(duplicateWorked, "Duplicates should not be able to be added");
        Assert.AreEqual(3, bracketsAfter, "Three brackets should be in the event after being called");

        foreach (IBracketBuilder builder in builders)
            Assert.IsTrue(ev.Brackets.ContainsKey(builder.Id), "All brackets should be in the event");
    }

    [TestMethod]
    public void RemoveBracketTest()
    {
        // Arrange
        Event ev = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        IBracketBuilder[] builders = new IBracketBuilder[3];
        for (int i = 0; i < builders.Length; i++)
        {
            IBracketBuilder builder = new SingleEliminationBuilder(
                SnowflakeService.Generate().ToString(),
                SnowflakeService);

            builders[i] = builder;
            ev.AddBracket(builder);
        }

        // Act
        int bracketsBefore = ev.Brackets.Count;

        bool anyFailed = false;
        foreach (IBracketBuilder builder in builders)
        {
            bool worked = ev.RemoveBracket(builder.Id);

            if (!worked)
                anyFailed = false;
        }

        bool duplicateWorked = ev.RemoveBracket(builders[0].Id);

        int bracketsAfter = ev.Brackets.Count;

        // Assert
        Assert.AreEqual(3, bracketsBefore, "The event should start with all 3 added brackets");
        Assert.IsFalse(anyFailed, "No valid builders should fail");
        Assert.IsFalse(duplicateWorked, "Invalid IDs should not be successfully removed");
        Assert.AreEqual(0, bracketsAfter, "All bracket should have been removed");
    }

    [TestMethod]
    public void SetIconTest()
    {
        // Arrange
        Event ev = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        string wrongLengthIcon = "12345678901234567";
        string invalidCharactersIcon = "123456789012345.";
        string validIcon = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthIconWorked = ev.SetIcon(wrongLengthIcon);
        bool invalidCharactersIconWorked = ev.SetIcon(invalidCharactersIcon);

        string? iconBeforeExpectedValids = ev.Icon;

        bool nullIconWorked = ev.SetIcon(null);
        bool validIconWorked = ev.SetIcon(validIcon);

        // Assert
        Assert.IsFalse(wrongLengthIconWorked, "An icon of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersIconWorked, "An icon with invalid characters shouldn't work");
        Assert.IsTrue(nullIconWorked, "A null value should work");
        Assert.IsTrue(validIconWorked, "This icon should be working");
        Assert.AreEqual(Event.DefaultIcon, iconBeforeExpectedValids, "The icon should not have changed from invalid icons");
        Assert.AreEqual(validIcon, ev.Icon, "Successful changes should update the icon");
    }

    [TestMethod]
    public void SetBannerTest()
    {
        // Arrange
        Event ev = new(
            SnowflakeService.Generate().ToString(),
            "Event",
            Tournament,
            DateTime.UtcNow);

        string wrongLengthBanner = "12345678901234567";
        string invalidCharactersBanner = "123456789012345.";
        string validBanner = "abcdef7890ABCDEF";

        // Act
        bool wrongLengthBannerWorked = ev.SetBanner(wrongLengthBanner);
        bool invalidCharactersBannerWorked = ev.SetBanner(invalidCharactersBanner);

        string? bannerBeforeExpectedValids = ev.Banner;

        bool nullBannerWorked = ev.SetBanner(null);
        bool validBannerWorked = ev.SetBanner(validBanner);

        // Assert
        Assert.IsFalse(wrongLengthBannerWorked, "An banner of the wrong length shouldn't work");
        Assert.IsFalse(invalidCharactersBannerWorked, "An banner with invalid characters shouldn't work");
        Assert.IsTrue(nullBannerWorked, "A null value should work");
        Assert.IsTrue(validBannerWorked, "This banner should be working");
        Assert.AreEqual(Event.DefaultBanner, bannerBeforeExpectedValids, "The banner should not have changed from invalid banners");
        Assert.AreEqual(validBanner, ev.Banner, "Successful changes should update the banner");
    }
}
