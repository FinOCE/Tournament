namespace API.Tests.Unit.Utils.Authentication;

[TestClass]
public class TokenTest
{
    SnowflakeService SnowflakeService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        SnowflakeService = new();

        // Set environment variable for unit tests during development
        Environment.SetEnvironmentVariable("AUTHORIZATION_SECRET", "DevelopmentAuthorizationSecret");
    }

    [TestMethod]
    public void ConstructorTest()
    {
        // Act
        Token invalidId() => new("");
        Token valid() => new(SnowflakeService.Generate().ToString());

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not work");
        Assert.IsInstanceOfType(valid(), typeof(Token), "A valid constructor should work");
    }

    [TestMethod]
    public void ToStringTest()
    {
        // Arrange
        Token basicToken = new("12345");
        Token fullToken = new(
            "12345",
            "issuer",
            "subject",
            "audience",
            new(2222, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
            new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
            new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        string basicJwtExpected = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImp0aSI6ICIxMjM0NSIgfSA=.tlYHOfL8CuHJO9u3qz4+YK0F4KZx0B1dVVpqSYmQHuM=";
        string fullJwtExpected = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImlzcyI6ICJpc3N1ZXIiLCAic3ViIjogInN1YmplY3QiLCAiYXVkIjogImF1ZGllbmNlIiwgImV4cCI6IDc5NTIzNDI0MDAsICJuYmYiOiAwLCAiaWF0IjogMCwgImp0aSI6ICIxMjM0NSIgfSA=.tncXoQxsGJLVNbtqTORRGkpQpKR6JdyTumrClTiqOqA=";

        // Act
        string basicJwt = basicToken.ToString();
        string fullJwt = fullToken.ToString();

        // Assert
        Assert.AreEqual(basicJwtExpected, basicJwt, "The token should match the expected");
        Assert.AreEqual(fullJwtExpected, fullJwt, "The token should match the expected");
    }

    [TestMethod]
    public void DeserializeTest()
    {
        // Arrange
        string invalidIdJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImlzcyI6ICJpc3N1ZXIiLCAic3ViIjogInN1YmplY3QiLCAiYXVkIjogImF1ZGllbmNlIiwgImV4cCI6IDAsICJuYmYiOiAwLCAiaWF0IjogMCwgImp0aSI6ICIiIH0g.PMgT4BH+o2sQEJnqzLWHXspKiA/9aU4lgf6UZp2TrwU=";
        string invalidFormatJwt = "";
        string malformedJsonJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.eyAianRpIjogIjEyMzQ1IiA=.fLCgTg6fmScprhA3Kl3XCV56D/CpQHOMbMEDlKchWw0=";
        string stringNotNumberJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImp0aSI6ICIxMjM0NSIsICJpYXQiOiAiaW52YWxpZCIgfSA=.4oaPFa3z+XvO1JccAxC7beoIwIPEvX07C10mvTa6d9s=";
        string numberNotStringJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImp0aSI6IDEyMzQ1IH0g.Vw2ir2jPJBeZpkVdLU1bDTrTYuISVNAQcQVERKIXgxk=";
        string validJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImlzcyI6ICJpc3N1ZXIiLCAic3ViIjogInN1YmplY3QiLCAiYXVkIjogImF1ZGllbmNlIiwgImV4cCI6IDc5NTIzNDI0MDAwMDAsICJuYmYiOiAwLCAiaWF0IjogMCwgImp0aSI6ICIxMjM0NSIgfSA=.ElCUQMEbrYQgnre5nI7B7dNlqu19Wlj9VAq35a4ADxU=";
        string validMinimalJwt = "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==.IHsgImp0aSI6ICIxMjM0NSIgfSA=.tlYHOfL8CuHJO9u3qz4+YK0F4KZx0B1dVVpqSYmQHuM=";

        // Act
        Token invalidId() => Token.Deserialize(invalidIdJwt);
        Token invalidFormat() => Token.Deserialize(invalidFormatJwt);
        Token malformedJson() => Token.Deserialize(malformedJsonJwt);
        Token stringNotNumber() => Token.Deserialize(stringNotNumberJwt);
        Token numberNotString() => Token.Deserialize(numberNotStringJwt);
        Token valid() => Token.Deserialize(validJwt);
        Token validMinimal() => Token.Deserialize(validMinimalJwt);

        // Assert
        Assert.ThrowsException<ArgumentException>(invalidId, "An invalid ID should not work");
        Assert.ThrowsException<ArgumentException>(invalidFormat, "A token with an invalid format should not work");
        Assert.ThrowsException<ArgumentException>(malformedJson, "Malformed JSON should not work");
        Assert.ThrowsException<ArgumentException>(stringNotNumber, "A string where a number should be should not work");
        Assert.ThrowsException<ArgumentException>(numberNotString, "A number where a string should be should not work");
        Assert.IsInstanceOfType(valid(), typeof(Token), "A complete valid constructor should work");
        Assert.IsInstanceOfType(validMinimal(), typeof(Token), "A minimal valid constructor should work");

        Assert.AreEqual("12345", valid().Payload.Id, "The ID should be available in the token");
        Assert.IsInstanceOfType(valid().Payload.IssuedAt, typeof(DateTime), "DateTimes should be available");
        Assert.IsNull(validMinimal().Payload.Expiration, "Unspecified properties should be null");
    }
}
