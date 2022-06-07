namespace API.Tests.Unit.Models.Attributes;

[TestClass]
public class VerifiableTest
{
    public class Implementation : Verifiable { }

    [TestMethod]
    public void VerifyTest()
    {
        // Arrange
        IVerifiable implementation = new Implementation();

        // Act
        bool isInitiallyVerified = implementation.Verified;
        implementation.Verify();
        bool isAfterCallVerified = implementation.Verified;
        implementation.Verify();
        bool isAfterSecondCallVerified = implementation.Verified;

        // Assert
        Assert.IsFalse(isInitiallyVerified, "The implementation should not be verified by default");
        Assert.IsTrue(isAfterCallVerified, "The implementation should be verified after call");
        Assert.IsTrue(isAfterSecondCallVerified, "The implementation should still be verified after second call");
    }
}
