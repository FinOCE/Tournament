namespace API.Tests.Unit.Utils.Authentication;

[TestClass]
public class HeaderTest
{
    [TestMethod]
    public void ToStringTest()
    {
        // Arrange
        Header header = new();

        // Act
        string json = header.ToString();

        // Assert
        Assert.AreEqual(
            "IHsgInR5cCI6ICJKV1QiLCAiYWxnIjogIkhTMjU2IiB9IA==",
            json,
            "The header should match what is generated manually");
    }
}
