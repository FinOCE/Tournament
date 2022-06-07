namespace API.Tests.Unit.Models.Socials;

[TestClass]
public class SocialTest
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
        string sg() => SnowflakeService.Generate().ToString(); // Shortened because it's used a lot
        string link = "https://example.com"; // Shortened because it's used a lot

        // Act
        Social invalidId() => new("", SocialPlatform.Website, link);
        Social invalidPlatform() => new(sg(), (SocialPlatform)10, link);
        Social invalidLink() => new(sg(), SocialPlatform.Website, "");
        Social valid() => new(sg(), SocialPlatform.Website, link);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID shouldn't work");
        Assert.ThrowsException<ArgumentException>(invalidPlatform, "An invalid platform shouldn't work");
        Assert.ThrowsException<ArgumentException>(invalidLink, "An invalid link shouldn't work");
        Assert.IsInstanceOfType(valid(), typeof(Social), "The model should be successfully created");
    }

    [TestMethod]
    public void SetLinkTest()
    {
        // Arrange
        Social social = new(SnowflakeService.Generate().ToString(), SocialPlatform.Website, "https://example.com");

        // Act
        string linkBeforeChanges = social.Link;

        bool invalidLink = social.SetLink("This is wrong");

        string linkAfterInvalidChanges = social.Link;

        bool usingHttpWorked = social.SetLink("http://unsafe.com");
        bool simpleUrlWorked = social.SetLink("https://example.com");
        bool withParamsWorked = social.SetLink("https://youtube.com/this_should-work?with=parameters&as=well");
        bool withDifferentTldWorked = social.SetLink("https://itsf.in");
        bool withTrailingWorked = social.SetLink("https://example.com/");
        bool nestedSubdomainWorked = social.SetLink("https://some.subdomain.at.website.com");

        string linkAfterChanges = social.Link;

        // Assert
        Assert.AreEqual("https://example.com", linkBeforeChanges, "The link should be what was instantiated");
        Assert.IsFalse(invalidLink, "A non-link shouldn't work");
        Assert.AreEqual("https://example.com", linkAfterInvalidChanges, "The link should not have changed");
        Assert.IsTrue(usingHttpWorked, "A HTTP site should work");
        Assert.IsTrue(simpleUrlWorked, "A simple URL should work");
        Assert.IsTrue(withParamsWorked, "URL Params should work");
        Assert.IsTrue(withDifferentTldWorked, "Different TLDs should work");
        Assert.IsTrue(withTrailingWorked, "Trailing slash should work");
        Assert.IsTrue(nestedSubdomainWorked, "Nested subdomains should work");
        Assert.AreEqual("https://some.subdomain.at.website.com", linkAfterChanges, "The link should match the last valid call");
    }

    [TestMethod]
    public void GetPlatformIcon()
    {
        // Act
        string discordIcon = Social.GetPlatformIcon(SocialPlatform.Discord);
        string websiteIcon = Social.GetPlatformIcon(SocialPlatform.Website);
        string youtubeIcon = Social.GetPlatformIcon(SocialPlatform.YouTube);

        // Assert
        Assert.AreEqual("1234567890123456", discordIcon, "The icons should match"); // TODO: Create platform icon paths
        Assert.AreEqual("1234567890123456", websiteIcon, "The icons should match");
        Assert.AreEqual("1234567890123456", youtubeIcon, "The icons should match");
    }
}
