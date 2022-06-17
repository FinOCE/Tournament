namespace API.Tests.Unit.Utils.Authentication;

[TestClass]
public class PayloadTest
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
        // Act
        Payload invalidId() => new("");
        Payload valid() => new(SnowflakeService.Generate().ToString());

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not work");
        Assert.IsInstanceOfType(valid(), typeof(Payload), "A valid call should instantiate the payload");
    }

    [TestMethod]
    public void SetExpirationTest()
    {
        // Arrange
        Payload payload = new(SnowflakeService.Generate().ToString());

        // Act
        DateTime? before = payload.Expiration;

        payload.SetExpiration(new(2022, 1, 1));

        DateTime? after = payload.Expiration;

        // Assert
        Assert.IsNull(before, "The expiration should be null by default");
        Assert.AreEqual(new DateTime(2022, 1, 1), after, "The expiration should have updated");
    }

    [TestMethod]
    public void SetNotBeforeTest()
    {
        // Arrange
        Payload payload = new(SnowflakeService.Generate().ToString());

        // Act
        DateTime? before = payload.NotBefore;

        payload.SetNotBefore(new(2022, 1, 1));

        DateTime? after = payload.NotBefore;

        // Assert
        Assert.IsNull(before, "The activation time should be null by default");
        Assert.AreEqual(new DateTime(2022, 1, 1), after, "The activation time should have updated");
    }

    [TestMethod]
    public void SetIssuedAtTest()
    {
        // Arrange
        Payload payload = new(SnowflakeService.Generate().ToString());

        // Act
        DateTime? before = payload.IssuedAt;

        payload.SetIssuedAt(new(2022, 1, 1));

        DateTime? after = payload.IssuedAt;

        // Assert
        Assert.IsNull(before, "The issue time should be null by default");
        Assert.AreEqual(new DateTime(2022, 1, 1), after, "The issue time should have updated");
    }

    [TestMethod]
    public void ToStringTest()
    {
        // Arrange
        Payload minimal = new("12345");
        Payload complete = new(
            "12345",
            "issuer",
            "subject",
            "audience",
            new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
            new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
            new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        // Act
        string minimalJson = minimal.ToString();
        string completeJson = complete.ToString();

        // Assert
        Assert.AreEqual(
            "IHsgImp0aSI6ICIxMjM0NSIgfSA=",
            minimalJson,
            "The minimal JSON should match the expected manually generated string");

        Assert.AreEqual(
            "IHsgImlzcyI6ICJpc3N1ZXIiLCAic3ViIjogInN1YmplY3QiLCAiYXVkIjogImF1ZGllbmNlIiwgImV4cCI6IDAsICJuYmYiOiAwLCAiaWF0IjogMCwgImp0aSI6ICIxMjM0NSIgfSA=",
            completeJson,
            "The complete JSON should match the expected manually generated string");
    }
}
