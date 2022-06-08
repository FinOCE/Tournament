namespace API.Tests.Unit.Models.Fragments;

[TestClass]
public class SociableTest
{
    public class Implementation : Sociable { }

    SnowflakeService SnowflakeService = null!;
    Social[] ExampleSocials = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        ExampleSocials = new Social[]
        {
            new(
                SnowflakeService.Generate().ToString(),
                SocialPlatform.Website,
                "https://example.com"),
            new(
                SnowflakeService.Generate().ToString(),
                SocialPlatform.Discord,
                "https://discord.gg/ExampleInvite"),
            new(
                SnowflakeService.Generate().ToString(),
                SocialPlatform.YouTube,
                "https://youtube.com/c/ExampleChannel")
        };
    }

    [TestMethod]
    public void SetSocialsTest()
    {
        // Arrange
        ISociable implementation = new Implementation();

        Dictionary<string, Social> validSocials = new();
        foreach (Social social in ExampleSocials)
            validSocials.Add(social.Id, social);

        Random random = new();

        Dictionary<string, Social> invalidSocials = new();
        foreach (Social social in ExampleSocials)
            invalidSocials.Add(random.Next(1, 9999999).ToString(), social);

        // Act
        implementation.SetSocials(validSocials);
        void setInvalid() => implementation.SetSocials(invalidSocials);


        // Assert
        Assert.AreEqual(ExampleSocials.Length, implementation.Socials.Count, "All socials should be set");
        Assert.ThrowsException<ArgumentException>(setInvalid, "Invalid socials should not be accepted");

        foreach (Social social in ExampleSocials)
            Assert.IsTrue(
                implementation.Socials.ContainsKey(social.Id),
                "All socials should be contained in the dictionary");
    }

    [TestMethod]
    public void AddSocialTest()
    {
        // Arrange
        ISociable implementation = new Implementation();

        // Act
        int socialsBeforeAdding = implementation.Socials.Count;

        foreach (Social social in ExampleSocials)
            implementation.AddSocial(social);

        bool duplicateAddWorked = implementation.AddSocial(ExampleSocials[0]);

        int socialsAfterAdding = implementation.Socials.Count;

        // Assert
        Assert.AreEqual(0, socialsBeforeAdding, "There should be no socials initially");
        Assert.AreEqual(ExampleSocials.Length, socialsAfterAdding, "All socials should have been added");
        Assert.IsFalse(duplicateAddWorked, "Duplicate socials should not get added");

        foreach (Social social in ExampleSocials)
            Assert.IsTrue(
                implementation.Socials.ContainsKey(social.Id),
                "All socials should be contained in the dictionary");
    }

    [TestMethod]
    public void RemoveSocialTest()
    {
        // Arrange
        ISociable implementation = new Implementation();

        foreach (Social social in ExampleSocials)
            implementation.AddSocial(social);

        // Act
        int socialsBeforeRemoving = implementation.Socials.Count;

        foreach (Social social in ExampleSocials)
            implementation.RemoveSocial(social.Id);

        bool duplicatedRemoveWorked = implementation.RemoveSocial(ExampleSocials[0].Id);

        int socialsAfterRemoving = implementation.Socials.Count;

        // Assert
        Assert.AreEqual(ExampleSocials.Length, socialsBeforeRemoving, "All example socials should be included initially");
        Assert.AreEqual(0, socialsAfterRemoving, "All socials should have been removed");
        Assert.IsFalse(duplicatedRemoveWorked, "Socials should not accept a second removal");
    }
}
